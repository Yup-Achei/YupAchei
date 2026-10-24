using YupAchei.Domain.Entities;
using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;
using YupAchei.Domain.ValueObjects;

namespace YupAchei.Domain.Tests;

public class EventTests
{
    private static readonly DateTimeOffset DataInicioPadrao = new(2026, 11, 21, 20, 0, 0, TimeSpan.FromHours(-4));
    private static readonly DateTimeOffset DataFimPadrao = new(2026, 11, 22, 1, 0, 0, TimeSpan.FromHours(-4));

    [Fact]
    public void Evento_ParaLugarCadastrado_DeveSerValidoComLugarId()
    {
        var lugarId = Guid.NewGuid();
        var organizador = new OrganizadorEvento("Produtora Cultural PVH", TipoDocumentoOrganizador.Cnpj, "00.000.000/0001-03");

        var evento = Evento.CriarParaLugar(
            titulo: "Feira Gastronômica Beira-Rio",
            descricao: "Food trucks, comida regional e música ao vivo no Complexo da EFMM.",
            dataInicio: DataInicioPadrao,
            dataFim: DataFimPadrao,
            lugarId: lugarId,
            faixaPreco: PriceRange.Gratis(),
            ticketUrl: null,
            etiquetasClima: ["gratuito", "familia", "por-do-sol", "musica-ao-vivo"],
            slug: "feira-gastronomica-efmm",
            categoria: CategoriaLugar.Gastronomia,
            organizador: organizador,
            destaque: true,
            patrocinado: true);

        Assert.True(evento.PossuiVinculoLugar);
        Assert.False(evento.PossuiLocalLivre);
        Assert.Equal("Feira Gastronômica Beira-Rio", evento.Titulo);
        Assert.Equal(CategoriaLugar.Gastronomia, evento.Categoria);
        Assert.Equal(lugarId, evento.LugarId);
        Assert.Null(evento.LocalLivre);
        Assert.True(evento.EstaAtivoEm(DataInicioPadrao.AddHours(1)));
        Assert.True(evento.EstaDisponivelNaAgendaEm(DataInicioPadrao.AddDays(-5)));
        Assert.False(evento.EstaDisponivelNaAgendaEm(DataFimPadrao.AddMinutes(1)));
    }

    [Fact]
    public void Evento_ParaLocalLivre_DeveSuportarLocalSemEnderecoOuBairroFormal()
    {
        // Caso real de eventos.json (evt-008: Forró nos Mirantes)
        var localLivre = new LocalLivre(
            nome: "Mirantes I, II e III",
            endereco: null,
            bairro: null,
            coordenadas: new GeoCoordinates(-8.7590, -63.9110));

        var evento = Evento.CriarParaLocalLivre(
            titulo: "Forró nos Mirantes",
            descricao: "Forró com vista para o Rio Madeira.",
            dataInicio: DataInicioPadrao,
            dataFim: DataFimPadrao,
            localLivre: localLivre,
            faixaPreco: PriceRange.APartirDe(20m),
            etiquetasClima: ["musica-ao-vivo", "vista-rio"],
            slug: "forro-nos-mirantes",
            categoria: CategoriaLugar.Festas);

        Assert.False(evento.PossuiVinculoLugar);
        Assert.True(evento.PossuiLocalLivre);
        Assert.Null(evento.LugarId);
        Assert.Equal("Mirantes I, II e III", evento.NomeLocal);
        Assert.Equal(localLivre, evento.LocalLivre);
    }

    [Fact]
    public void Evento_DeveRejeitarCategoriaExclusivaDeLugar()
    {
        Assert.Throws<DomainException>(() =>
            Evento.CriarParaLugar(
                titulo: "Evento com Categoria Inválida",
                descricao: "Descrição",
                dataInicio: DataInicioPadrao,
                dataFim: DataFimPadrao,
                lugarId: Guid.NewGuid(),
                faixaPreco: PriceRange.Gratis(),
                categoria: CategoriaLugar.Cafeterias));
    }

    [Fact]
    public void Evento_DeveLancarDomainException_QuandoDataFimNaoForPosteriorADataInicio()
    {
        Assert.Throws<DomainException>(() =>
            Evento.CriarParaLugar(
                "Show Inválido",
                "Descrição",
                DataInicioPadrao,
                DataInicioPadrao.AddHours(-1),
                Guid.NewGuid(),
                PriceRange.Gratis()));
    }
}
