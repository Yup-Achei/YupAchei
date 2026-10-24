using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;
using YupAchei.Domain.ValueObjects;

namespace YupAchei.Domain.Tests;

public class ValueObjectsTests
{
    [Theory]
    [InlineData(-91, -63.9)]
    [InlineData(91, -63.9)]
    [InlineData(-8.76, -181)]
    [InlineData(-8.76, 181)]
    public void GeoCoordinates_DeveLancarDomainException_QuandoForaDosLimites(double latitude, double longitude)
    {
        Assert.Throws<DomainException>(() => new GeoCoordinates(latitude, longitude));
    }

    [Fact]
    public void GeoCoordinates_CalcularDistanciaEmKmPara_DeveCalcularDistanciaPrecisaEmPortoVelho()
    {
        // Centro de Porto Velho (~ -8.7619, -63.9039) até Porto Velho Shopping (~ -8.7475, -63.8768)
        var centro = new GeoCoordinates(-8.7619, -63.9039);
        var shopping = new GeoCoordinates(-8.7475, -63.8768);

        var distanciaKm = centro.CalcularDistanciaEmKmPara(shopping);

        Assert.InRange(distanciaKm, 3.0, 3.8);
    }

    [Fact]
    public void WeeklySchedule_EstaAbertoEm_DeveSuportarHorariosComerciaisEViradaDeMeiaNoite()
    {
        var agenda = new WeeklySchedule(
        [
            new DailyHours(DayOfWeek.Friday, new TimeOnly(18, 0), new TimeOnly(2, 0)), // Fecha às 02:00 de sábado
            new DailyHours(DayOfWeek.Saturday, new TimeOnly(9, 0), new TimeOnly(17, 0))
        ]);

        // Sexta às 21:00 -> Aberto
        var sextaNoite = new DateTimeOffset(2026, 10, 16, 21, 0, 0, TimeSpan.FromHours(-4));
        Assert.True(agenda.EstaAbertoEm(sextaNoite));

        // Sábado à 01:30 da manhã (ainda no turno de sexta-feira) -> Aberto
        var sabadoMadrugada = new DateTimeOffset(2026, 10, 17, 1, 30, 0, TimeSpan.FromHours(-4));
        Assert.True(agenda.EstaAbertoEm(sabadoMadrugada));

        // Sábado às 03:00 da manhã -> Fechado
        var sabadoAposFechamento = new DateTimeOffset(2026, 10, 17, 3, 0, 0, TimeSpan.FromHours(-4));
        Assert.False(agenda.EstaAbertoEm(sabadoAposFechamento));

        // Sábado às 12:00 -> Aberto
        var sabadoMeioDia = new DateTimeOffset(2026, 10, 17, 12, 0, 0, TimeSpan.FromHours(-4));
        Assert.True(agenda.EstaAbertoEm(sabadoMeioDia));
    }

    [Fact]
    public void Address_DevePermitirLogradouroOuBairroNulosParaLocaisPublicos_MasExigirCidadeEUf()
    {
        // Praça Aluízio Ferreira (logradouro nulo) e Espaço Alternativo (bairro nulo)
        var pracaAluizio = new Address(null, "s/n", "Centro", UrbanRegion.Centro, "Porto Velho", "RO");
        var espacoAlternativo = new Address("Av. Jorge Teixeira", "s/n", null, null, "Porto Velho", "RO");

        Assert.Null(pracaAluizio.Logradouro);
        Assert.Equal("Centro", pracaAluizio.Bairro);
        Assert.Null(espacoAlternativo.Bairro);
        Assert.Equal("Porto Velho", espacoAlternativo.Cidade);

        Assert.Throws<DomainException>(() =>
            new Address("Av. Sete de Setembro", "100", "Centro", UrbanRegion.Centro, "", "RO"));
        Assert.Throws<DomainException>(() =>
            new Address("Av. Sete de Setembro", "100", "Centro", UrbanRegion.Centro, "Porto Velho", "RONDONIA"));
    }

    [Fact]
    public void PriceRange_DeveSuportarGratis_APartirDe_ERejeitarValoresInvalidos()
    {
        Assert.Throws<DomainException>(() => new PriceRange(-10m, 50m));
        Assert.Throws<DomainException>(() => new PriceRange(100m, 50m));

        var gratuito = PriceRange.Gratis();
        Assert.True(gratuito.Gratuito);
        Assert.Equal(0m, gratuito.ValorMinimo);
        Assert.Equal("BRL", gratuito.Moeda);

        var aPartirDe = PriceRange.APartirDe(30m);
        Assert.False(aPartirDe.Gratuito);
        Assert.Equal(30m, aPartirDe.ValorMinimo);
        Assert.Equal("BRL", aPartirDe.Moeda);
    }
}
