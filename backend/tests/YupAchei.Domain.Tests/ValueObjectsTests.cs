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
    public void GeoCoordinates_ShouldThrowDomainException_WhenOutOfBounds(double latitude, double longitude)
    {
        Assert.Throws<DomainException>(() => new GeoCoordinates(latitude, longitude));
    }

    [Fact]
    public void GeoCoordinates_CalculateDistanceInKmTo_ShouldCalculateAccurateDistanceInPortoVelho()
    {
        // Downtown Porto Velho (~ -8.7619, -63.9039) to Porto Velho Shopping (~ -8.7475, -63.8768)
        var downtown = new GeoCoordinates(-8.7619, -63.9039);
        var shopping = new GeoCoordinates(-8.7475, -63.8768);

        var distanceKm = downtown.DistanceInKilometersTo(shopping);

        Assert.InRange(distanceKm, 3.0, 3.8);
    }

    [Fact]
    public void WeeklySchedule_IsOpenAt_ShouldSupportRegularAndOvernightHours()
    {
        var schedule = new WeeklySchedule(
        [
            new DailyHours(DayOfWeek.Friday, new TimeOnly(18, 0), new TimeOnly(2, 0)), // Closes at 02:00 on Saturday
            new DailyHours(DayOfWeek.Saturday, new TimeOnly(9, 0), new TimeOnly(17, 0))
        ]);

        // Friday at 21:00 -> Open
        var fridayNight = new DateTimeOffset(2026, 10, 16, 21, 0, 0, TimeSpan.FromHours(-4));
        Assert.True(schedule.IsOpenAt(fridayNight));

        // Saturday at 01:30 AM (still in Friday's overnight shift) -> Open
        var saturdayEarlyMorning = new DateTimeOffset(2026, 10, 17, 1, 30, 0, TimeSpan.FromHours(-4));
        Assert.True(schedule.IsOpenAt(saturdayEarlyMorning));

        // Saturday at 03:00 AM -> Closed
        var saturdayAfterClose = new DateTimeOffset(2026, 10, 17, 3, 0, 0, TimeSpan.FromHours(-4));
        Assert.False(schedule.IsOpenAt(saturdayAfterClose));

        // Saturday at 12:00 PM -> Open
        var saturdayNoon = new DateTimeOffset(2026, 10, 17, 12, 0, 0, TimeSpan.FromHours(-4));
        Assert.True(schedule.IsOpenAt(saturdayNoon));
    }

    [Fact]
    public void Address_ShouldAllowNullStreetOrNeighborhoodForPublicSpaces_ButRequireCityAndState()
    {
        // Praça Aluízio Ferreira (null street) and Espaço Alternativo (null neighborhood)
        var pracaAluizio = new Address(null, "s/n", "Centro", UrbanRegion.Center, "Porto Velho", "RO");
        var espacoAlternativo = new Address("Av. Jorge Teixeira", "s/n", null, null, "Porto Velho", "RO");

        Assert.Null(pracaAluizio.Street);
        Assert.Equal("Centro", pracaAluizio.Neighborhood);
        Assert.Null(espacoAlternativo.Neighborhood);
        Assert.Equal("Porto Velho", espacoAlternativo.City);

        Assert.Throws<DomainException>(() =>
            new Address("Av. Sete de Setembro", "100", "Centro", UrbanRegion.Center, "", "RO"));
        Assert.Throws<DomainException>(() =>
            new Address("Av. Sete de Setembro", "100", "Centro", UrbanRegion.Center, "Porto Velho", "RONDONIA"));
    }

    [Fact]
    public void PriceRange_ShouldSupportFree_StartingFrom_AndRejectInvalidValues()
    {
        Assert.Throws<DomainException>(() => new PriceRange(-10m, 50m));
        Assert.Throws<DomainException>(() => new PriceRange(100m, 50m));

        var free = PriceRange.Free();
        Assert.True(free.IsFree);
        Assert.Equal(0m, free.MinAmount);
        Assert.Equal("BRL", free.Currency);

        var startingFrom = PriceRange.StartingFrom(30m);
        Assert.False(startingFrom.IsFree);
        Assert.Equal(30m, startingFrom.MinAmount);
        Assert.Equal("BRL", startingFrom.Currency);
    }
}
