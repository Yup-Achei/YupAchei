using YupAchei.Domain.Entities;
using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;
using YupAchei.Domain.ValueObjects;

namespace YupAchei.Domain.Tests;

public class PlaceAndReviewsTests
{
    private static Place CreateSamplePlace(WeeklySchedule? schedule = null, bool isOpen24Hours = false)
    {
        return new Place(
            name: "Café Madeira",
            slug: "cafe-madeira",
            description: "Cafeteria artesanal no coração de Porto Velho.",
            category: PlaceCategory.Cafes,
            address: new Address("Av. Carlos Gomes", "1234", "Centro", UrbanRegion.Center, "Porto Velho", "RO", "76801-000"),
            coordinates: new GeoCoordinates(-8.7619, -63.9039),
            openingHours: schedule ?? WeeklySchedule.Empty(),
            links: new UsefulLinks("https://maps.google.com/?q=cafe", "@cafemadeira", "5569999999999"),
            vibeTags: ["familia", "climatizado", "wifi"],
            photos: [new MediaAsset("https://cdn.yupachei.com/cafe-madeira.webp", "Fachada", isCover: true)],
            kind: PlaceKind.Commercial,
            priceRange: new PriceRange(18m, 55m),
            isOpen24Hours: isOpen24Hours
        );
    }

    [Fact]
    public void Place_ShouldStartUnderReview_AndUseUnifiedPriceRangeAndSlug()
    {
        var place = CreateSamplePlace();

        Assert.Equal("Café Madeira", place.Name);
        Assert.Equal("cafe-madeira", place.Slug);
        Assert.Equal(PlaceCategory.Cafes, place.Category);
        Assert.Equal(PlaceKind.Commercial, place.Kind);
        Assert.Equal(18m, place.PriceRange.MinAmount);
        Assert.Equal(55m, place.PriceRange.MaxAmount);
        Assert.True(place.IsActive);
        Assert.Equal(PublicationStatus.UnderReview, place.Status);
        Assert.False(place.IsApproved);

        place.Approve();
        Assert.Equal(PublicationStatus.Approved, place.Status);
        Assert.True(place.IsApproved);
    }

    [Fact]
    public void Place_StatusTransitions_ShouldPreventApprovingDirectlyFromRejected()
    {
        var place = CreateSamplePlace();

        place.Reject();
        Assert.Equal(PublicationStatus.Rejected, place.Status);

        Assert.Throws<DomainException>(() => place.Approve());

        place.SendToReview();
        place.Approve();
        Assert.Equal(PublicationStatus.Approved, place.Status);
    }

    [Fact]
    public void Place_Open24Hours_ShouldBeOpenOnlyWhenApproved()
    {
        var square = CreateSamplePlace(schedule: WeeklySchedule.Empty(), isOpen24Hours: true);
        var earlyMorning = new DateTimeOffset(2026, 10, 24, 3, 30, 0, TimeSpan.FromHours(-4));

        // Starts under review -> not yet visible/open to the public
        Assert.False(square.IsOpenAt(earlyMorning));

        square.Approve();
        Assert.True(square.IsOpenAt(earlyMorning));

        square.SendToReview();
        Assert.False(square.IsOpenAt(earlyMorning));
    }

    [Fact]
    public void Place_ShouldRejectEventExclusiveCategory()
    {
        Assert.Throws<DomainException>(() => new Place(
            name: "Invalid Place",
            slug: "invalid-place",
            description: "Description",
            category: PlaceCategory.Parties,
            address: new Address("Rua A", "10", "Centro", UrbanRegion.Center, "Porto Velho", "RO"),
            coordinates: new GeoCoordinates(-8.76, -63.90)));
    }

    [Fact]
    public void Place_AddPartnerReview_ShouldReferencePartnerEntityWithoutDuplicatingData()
    {
        var place = CreateSamplePlace();
        var partner = new Partner(
            name: "Rondônia Gourmet",
            avatarUrl: "https://cdn.yupachei.com/avatars/rg.webp",
            socialNetwork: SocialNetwork.Instagram,
            socialProfileUrl: "https://instagram.com/rondoniagourmet");

        var partnerReview = PartnerReview.CreateWithPartner(
            placeId: place.Id,
            partner: partner,
            badgeOrScore: "Top Pick 4.9",
            shortComment: "Melhor espresso e pão de queijo da região central!",
            isFeatured: true);

        place.AddPartnerReview(partnerReview);

        Assert.Single(place.PartnerReviews);
        Assert.Equal(partner.Id, place.PartnerReviews[0].PartnerId);
        Assert.Equal("Rondônia Gourmet", place.PartnerReviews[0].Partner?.Name);

        var longComment = new string('a', 201);
        Assert.Throws<DomainException>(() => PartnerReview.CreateWithPartner(
            place.Id,
            partner,
            "5.0",
            longComment));
    }

    [Fact]
    public void UserReview_And_PlaceAverageRating_ShouldSupportAuthenticatedUserReviews()
    {
        var place = CreateSamplePlace();
        var user1 = new User("Ana Silva", "ana@email.com");
        var user2 = new User("Carlos Souza", "carlos@email.com");

        var review1 = new UserReview(place.Id, user1.Id, 5, "Excelente atendimento!");
        var review2 = new UserReview(place.Id, user2.Id, 4, "Muito bom.");

        place.ApplyUserReview(review1.Rating);
        place.ApplyUserReview(review2.Rating);

        Assert.Equal(2, place.ReviewCount);
        Assert.Equal(4.5, place.AverageRating);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void UserReview_ShouldThrowDomainException_WhenRatingIsOutside1To5Range(int invalidRating)
    {
        Assert.Throws<DomainException>(() =>
            new UserReview(Guid.NewGuid(), Guid.NewGuid(), invalidRating, "Comment"));
    }

    [Fact]
    public void User_ShouldManageSavedPlaces()
    {
        var user = new User("Rodrigo", "rodrigo@yupachei.com");
        var placeId = Guid.NewGuid();

        user.SavePlace(placeId);
        user.SavePlace(placeId);

        Assert.Single(user.SavedPlaceIds);
        Assert.Contains(placeId, user.SavedPlaceIds);

        user.RemoveSavedPlace(placeId);
        Assert.Empty(user.SavedPlaceIds);
    }
}
