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
            faixaPreco: 2,
            aberto24h: aberto24h
        );
    }

    [Fact]
    public void Lugar_DeveInicializarCorretamente_ENormalizarEtiquetasClima()
    {
        var lugar = CriarLugarExemplo();

        Assert.Equal("Café Madeira", lugar.Nome);
        Assert.Equal("cafe-madeira", lugar.Slogan);
        Assert.Equal(CategoriaLugar.Cafeterias, lugar.Categoria);
        Assert.Equal(TipoLugar.Comercial, lugar.Tipo);
        Assert.Equal(2, lugar.FaixaPreco);
        Assert.True(lugar.Ativo);
        Assert.True(lugar.EstaAprovado);
        Assert.Equal(3, lugar.EtiquetasClima.Count);
        Assert.Equal(0d, lugar.NotaMedia);
        Assert.Equal(0, lugar.QuantidadeAvaliacoes);
    }

    [Fact]
    public void Lugar_Aberto24h_DeveEstarAbertoMesmoComListaDeHorariosVazia()
    {
        // Exemplo real: Praça das Três Caixas d'Água (aberto24h: true, horarios: [])
        var praca = CriarLugarExemplo(agenda: WeeklySchedule.Vazia(), aberto24h: true);
        var madrugada = new DateTimeOffset(2026, 10, 24, 3, 30, 0, TimeSpan.FromHours(-4));

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
            categoria: CategoriaLugar.Festas, // Categoria exclusiva de evento
            endereco: new Address("Rua A", "10", "Centro", UrbanRegion.Centro, "Porto Velho", "RO"),
            coordenadas: new GeoCoordinates(-8.76, -63.90)));
    }

    [Fact]
    public void Lugar_AdicionarAvaliacaoParceiro_DeveAdicionarCuradoria_ERejeitarComentarioLongo()
    {
        var lugar = CriarLugarExemplo();

        var avaliacaoParceiro = new AvaliacaoParceiro(
            lugarId: lugar.Id,
            nomeParceiro: "Rondônia Gourmet",
            urlAvatar: "https://cdn.yupachei.com/avatars/rg.webp",
            redeSocial: SocialNetwork.Instagram,
            seloOuNota: "Top Escolha 4.9",
            comentarioCurto: "Melhor espresso e pão de queijo da região central!",
            emDestaque: true,
            urlPerfilSocial: "https://instagram.com/rondoniagourmet"
        );

        lugar.AdicionarAvaliacaoParceiro(avaliacaoParceiro);

        Assert.Single(lugar.AvaliacoesParceiros);
        Assert.Equal("Rondônia Gourmet", lugar.AvaliacoesParceiros[0].NomeParceiro);

        var comentarioLongo = new string('a', 201);
        Assert.Throws<DomainException>(() => new AvaliacaoParceiro(
            lugar.Id,
            "Influenciador",
            "https://cdn.yupachei.com/a.webp",
            SocialNetwork.TikTok,
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
        usuario.SalvarLugar(lugarId); // Idempotente

        Assert.Single(usuario.LugaresSalvosIds);
        Assert.Contains(lugarId, usuario.LugaresSalvosIds);

        usuario.RemoverLugarSalvo(lugarId);
        Assert.Empty(usuario.LugaresSalvosIds);
    }
}
