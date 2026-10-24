using YupAchei.Domain.Entities;
using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;
using YupAchei.Domain.ValueObjects;

namespace YupAchei.Domain.Tests;

public class PlaceAndReviewsTests
{
    private static Lugar CriarLugarExemplo(WeeklySchedule? agenda = null, bool aberto24h = false)
    {
        return new Lugar(
            nome: "Café Madeira",
            slogan: "cafe-madeira",
            descricao: "Cafeteria artesanal no coração de Porto Velho.",
            categoria: CategoriaLugar.Cafeterias,
            endereco: new Address("Av. Carlos Gomes", "1234", "Centro", UrbanRegion.Centro, "Porto Velho", "RO", "76801-000"),
            coordenadas: new GeoCoordinates(-8.7619, -63.9039),
            horarioFuncionamento: agenda ?? WeeklySchedule.Vazia(),
            links: new UsefulLinks("https://maps.google.com/?q=cafe", "@cafemadeira", "5569999999999"),
            etiquetasClima: ["familia", "climatizado", "wifi"],
            fotos: [new MediaAsset("https://cdn.yupachei.com/cafe-madeira.webp", "Fachada", ehCapa: true)],
            tipo: TipoLugar.Comercial,
            faixaPreco: new PriceRange(18m, 55m),
            aberto24h: aberto24h
        );
    }

    [Fact]
    public void Lugar_DeveNascerEmRevisao_EUsarPriceRangeUnificado()
    {
        var lugar = CriarLugarExemplo();

        Assert.Equal("Café Madeira", lugar.Nome);
        Assert.Equal("cafe-madeira", lugar.Slogan);
        Assert.Equal(CategoriaLugar.Cafeterias, lugar.Categoria);
        Assert.Equal(TipoLugar.Comercial, lugar.Tipo);
        Assert.Equal(18m, lugar.FaixaPreco.ValorMinimo);
        Assert.Equal(55m, lugar.FaixaPreco.ValorMaximo);
        Assert.True(lugar.Ativo);
        Assert.Equal(StatusPublicacao.EmRevisao, lugar.Status);
        Assert.False(lugar.EstaAprovado);

        lugar.Aprovar();
        Assert.Equal(StatusPublicacao.Aprovado, lugar.Status);
        Assert.True(lugar.EstaAprovado);
    }

    [Fact]
    public void Lugar_TransicoesDeStatus_DeveImpedirAprovarDiretoDeRejeitado()
    {
        var lugar = CriarLugarExemplo();

        lugar.Rejeitar();
        Assert.Equal(StatusPublicacao.Rejeitado, lugar.Status);

        Assert.Throws<DomainException>(() => lugar.Aprovar());

        lugar.ColocarEmRevisao();
        lugar.Aprovar();
        Assert.Equal(StatusPublicacao.Aprovado, lugar.Status);
    }

    [Fact]
    public void Lugar_Aberto24h_DeveEstarAbertoApenasQuandoAprovado()
    {
        var praca = CriarLugarExemplo(agenda: WeeklySchedule.Vazia(), aberto24h: true);
        var madrugada = new DateTimeOffset(2026, 10, 24, 3, 30, 0, TimeSpan.FromHours(-4));

        // Nasce em revisão -> ainda não aparece como aberto ao público
        Assert.False(praca.EstaAbertoEm(madrugada));

        praca.Aprovar();
        Assert.True(praca.EstaAbertoEm(madrugada));

        praca.ColocarEmRevisao();
        Assert.False(praca.EstaAbertoEm(madrugada));
    }

    [Fact]
    public void Lugar_DeveRejeitarCategoriaExclusivaDeEvento()
    {
        Assert.Throws<DomainException>(() => new Lugar(
            nome: "Local Inválido",
            slogan: "local-invalido",
            descricao: "Descrição",
            categoria: CategoriaLugar.Festas,
            endereco: new Address("Rua A", "10", "Centro", UrbanRegion.Centro, "Porto Velho", "RO"),
            coordenadas: new GeoCoordinates(-8.76, -63.90)));
    }

    [Fact]
    public void Lugar_AdicionarAvaliacaoParceiro_DeveVincularEntidadeParceiroSemDuplicarDados()
    {
        var lugar = CriarLugarExemplo();
        var parceiro = new Parceiro(
            nome: "Rondônia Gourmet",
            urlAvatar: "https://cdn.yupachei.com/avatars/rg.webp",
            redeSocial: SocialNetwork.Instagram,
            urlPerfilSocial: "https://instagram.com/rondoniagourmet");

        var avaliacaoParceiro = AvaliacaoParceiro.CriarComParceiro(
            lugarId: lugar.Id,
            parceiro: parceiro,
            seloOuNota: "Top Escolha 4.9",
            comentarioCurto: "Melhor espresso e pão de queijo da região central!",
            emDestaque: true);

        lugar.AdicionarAvaliacaoParceiro(avaliacaoParceiro);

        Assert.Single(lugar.AvaliacoesParceiros);
        Assert.Equal(parceiro.Id, lugar.AvaliacoesParceiros[0].ParceiroId);
        Assert.Equal("Rondônia Gourmet", lugar.AvaliacoesParceiros[0].Parceiro?.Nome);

        var comentarioLongo = new string('a', 201);
        Assert.Throws<DomainException>(() => AvaliacaoParceiro.CriarComParceiro(
            lugar.Id,
            parceiro,
            "5.0",
            comentarioLongo));
    }

    [Fact]
    public void AvaliacaoUser_E_CalculoNotaMediaLugar_DeveSuportarAvaliacoesDeUsuariosLogados()
    {
        var lugar = CriarLugarExemplo();
        var usuario1 = new User("Ana Silva", "ana@email.com");
        var usuario2 = new User("Carlos Souza", "carlos@email.com");

        var avaliacao1 = new AvaliacaoUser(lugar.Id, usuario1.Id, 5, "Excelente atendimento!");
        var avaliacao2 = new AvaliacaoUser(lugar.Id, usuario2.Id, 4, "Muito bom.");

        lugar.AplicarAvaliacaoUsuario(avaliacao1.Nota);
        lugar.AplicarAvaliacaoUsuario(avaliacao2.Nota);

        Assert.Equal(2, lugar.QuantidadeAvaliacoes);
        Assert.Equal(4.5, lugar.NotaMedia);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void AvaliacaoUser_DeveLancarDomainException_QuandoNotaForaDoIntervalo1A5(int notaInvalida)
    {
        Assert.Throws<DomainException>(() =>
            new AvaliacaoUser(Guid.NewGuid(), Guid.NewGuid(), notaInvalida, "Comentário"));
    }

    [Fact]
    public void User_DeveGerenciarLugaresSalvos()
    {
        var usuario = new User("Rodrigo", "rodrigo@yupachei.com");
        var lugarId = Guid.NewGuid();

        usuario.SalvarLugar(lugarId);
        usuario.SalvarLugar(lugarId);

        Assert.Single(usuario.LugaresSalvosIds);
        Assert.Contains(lugarId, usuario.LugaresSalvosIds);

        usuario.RemoverLugarSalvo(lugarId);
        Assert.Empty(usuario.LugaresSalvosIds);
    }
}
