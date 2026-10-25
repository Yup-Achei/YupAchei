using YupAchei.Domain.Entities;
using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;
using YupAchei.Domain.ValueObjects;

namespace YupAchei.Domain.Tests;

public class EventTests
{
    private static readonly DateTimeOffset DefaultStartDate = new(2026, 11, 21, 20, 0, 0, TimeSpan.FromHours(-4));
    private static readonly DateTimeOffset DefaultEndDate = new(2026, 11, 22, 1, 0, 0, TimeSpan.FromHours(-4));

    [Fact]
    public void Event_ForRegisteredPlace_ShouldStartUnderReview_AndUseSlugAndDocumentFreeOrganizer()
    {
        var placeId = Guid.NewGuid();
        var organizer = new EventOrganizer("Produtora Cultural PVH", "@produtorapvh", isVerified: true);

        var @event = Event.CreateForPlace(
            title: "Feira Gastronômica Beira-Rio",
            description: "Food trucks, comida regional e música ao vivo no Complexo da EFMM.",
            startDate: DefaultStartDate,
            endDate: DefaultEndDate,
            placeId: placeId,
            priceRange: PriceRange.Free(),
            ticketUrl: null,
            vibeTags: ["gratuito", "familia", "por-do-sol", "musica-ao-vivo"],
            slug: "feira-gastronomica-efmm",
            category: PlaceCategory.Gastronomy,
            organizer: organizer,
            isFeatured: true,
            isSponsored: true);

        Assert.True(@event.HasPlaceLink);
        Assert.False(@event.HasFreeLocation);
        Assert.Equal("Feira Gastronômica Beira-Rio", @event.Title);
        Assert.Equal("feira-gastronomica-efmm", @event.Slug);
        Assert.Equal(PlaceCategory.Gastronomy, @event.Category);
        Assert.Equal(placeId, @event.PlaceId);
        Assert.Null(@event.FreeLocation);
        Assert.Equal(PublicationStatus.UnderReview, @event.Status);
        Assert.False(@event.IsAvailableInScheduleAt(DefaultStartDate.AddDays(-5)));

        @event.Approve();
        Assert.Equal(PublicationStatus.Approved, @event.Status);
        Assert.True(@event.IsActiveAt(DefaultStartDate.AddHours(1)));
        Assert.True(@event.IsAvailableInScheduleAt(DefaultStartDate.AddDays(-5)));
        Assert.False(@event.IsAvailableInScheduleAt(DefaultEndDate.AddMinutes(1)));
    }

    [Fact]
    public void Event_ForFreeLocation_ShouldSupportLocationWithoutFormalAddressOrNeighborhood()
    {
        var freeLocation = new FreeLocation(
            name: "Mirantes I, II e III",
            addressText: null,
            neighborhood: null,
            coordinates: new GeoCoordinates(-8.7590, -63.9110));

        var @event = Event.CreateForFreeLocation(
            title: "Forró nos Mirantes",
            description: "Forró com vista para o Rio Madeira.",
            startDate: DefaultStartDate,
            endDate: DefaultEndDate,
            freeLocation: freeLocation,
            priceRange: PriceRange.StartingFrom(20m),
            vibeTags: ["musica-ao-vivo", "vista-rio"],
            slug: "forro-nos-mirantes",
            category: PlaceCategory.Parties);

        Assert.False(@event.HasPlaceLink);
        Assert.True(@event.HasFreeLocation);
        Assert.Null(@event.PlaceId);
        Assert.Equal("forro-nos-mirantes", @event.Slug);
        Assert.Equal("Mirantes I, II e III", @event.VenueName);
        Assert.Equal(freeLocation, @event.FreeLocation);
    }

    [Fact]
    public void Event_ShouldRejectPlaceExclusiveCategory()
    {
        Assert.Throws<DomainException>(() =>
            Event.CreateForPlace(
                title: "Event with Invalid Category",
                description: "Description",
                startDate: DefaultStartDate,
                endDate: DefaultEndDate,
                placeId: Guid.NewGuid(),
                priceRange: PriceRange.Free(),
                category: PlaceCategory.Cafes));
    }

    [Fact]
    public void Event_ShouldThrowDomainException_WhenEndDateIsNotAfterStartDate()
    {
        Assert.Throws<DomainException>(() =>
            Event.CreateForPlace(
                "Invalid Show",
                "Description",
                DefaultStartDate,
                DefaultStartDate.AddHours(-1),
                Guid.NewGuid(),
                PriceRange.Free()));
    }
}
