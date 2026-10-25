using YupAchei.Domain.Common;
using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;
using YupAchei.Domain.ValueObjects;

namespace YupAchei.Domain.Entities;

public sealed class Place : AggregateRoot
{
    private static readonly HashSet<PlaceCategory> ValidPlaceCategories =
    [
        PlaceCategory.Cafes,
        PlaceCategory.Restaurants,
        PlaceCategory.Bars,
        PlaceCategory.Culture,
        PlaceCategory.Tours,
        PlaceCategory.Sports
    ];

    private readonly List<string> _vibeTags = [];
    private readonly List<MediaAsset> _photos = [];
    private readonly List<PartnerReview> _partnerReviews = [];

    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string Description { get; private set; }
    public PlaceCategory Category { get; private set; }
    public PlaceKind Kind { get; private set; }
    public Address Address { get; private set; }
    public GeoCoordinates Coordinates { get; private set; }
    public PriceRange PriceRange { get; private set; }
    public bool IsOpen24Hours { get; private set; }
    public WeeklySchedule OpeningHours { get; private set; }
    public string? Notes { get; private set; }
    public UsefulLinks Links { get; private set; }
    public double AverageRating { get; private set; }
    public int ReviewCount { get; private set; }
    public string RecommendedBy { get; private set; }
    public bool IsSponsored { get; private set; }
    public PublicationStatus Status { get; private set; }
    public PlaceVerification Verification { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public bool IsActive { get; private set; }

    public bool IsApproved => IsActive && Status == PublicationStatus.Approved;
    public IReadOnlyList<string> VibeTags => _vibeTags.AsReadOnly();
    public IReadOnlyList<MediaAsset> Photos => _photos.AsReadOnly();
    public IReadOnlyList<PartnerReview> PartnerReviews => _partnerReviews.AsReadOnly();

    public Place(
        string name,
        string slug,
        string description,
        PlaceCategory category,
        Address address,
        GeoCoordinates coordinates,
        WeeklySchedule? openingHours = null,
        UsefulLinks? links = null,
        IEnumerable<string>? vibeTags = null,
        IEnumerable<MediaAsset>? photos = null,
        PlaceKind kind = PlaceKind.Commercial,
        PriceRange? priceRange = null,
        bool isOpen24Hours = false,
        string? notes = null,
        string recommendedBy = "Equipe Yup Achei",
        bool isSponsored = false,
        PlaceVerification? verification = null,
        DateTimeOffset? updatedAt = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("O nome do lugar é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("O slug do lugar é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("A descrição do lugar é obrigatória.");
        }

        if (!ValidPlaceCategories.Contains(category))
        {
            throw new DomainException($"A categoria '{category}' não é válida para um Lugar.");
        }

        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(coordinates);

        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        Description = description.Trim();
        Category = category;
        Kind = kind;
        Address = address;
        Coordinates = coordinates;
        PriceRange = priceRange ?? PriceRange.Free();
        IsOpen24Hours = isOpen24Hours;
        OpeningHours = openingHours ?? WeeklySchedule.Empty();
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        Links = links ?? UsefulLinks.Empty();
        RecommendedBy = string.IsNullOrWhiteSpace(recommendedBy) ? "Equipe Yup Achei" : recommendedBy.Trim();
        IsSponsored = isSponsored;
        Status = PublicationStatus.UnderReview;
        Verification = verification ?? PlaceVerification.Unverified();
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
        IsActive = true;

        if (vibeTags is not null)
        {
            foreach (var tag in vibeTags.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                _vibeTags.Add(tag);
            }
        }

        if (photos is not null)
        {
            _photos.AddRange(photos);
        }
    }

    public bool IsOpenAt(DateTimeOffset timestamp)
    {
        if (!IsApproved)
        {
            return false;
        }

        if (IsOpen24Hours)
        {
            return true;
        }

        return OpeningHours.IsOpenAt(timestamp);
    }

    public void AddPartnerReview(PartnerReview review)
    {
        ArgumentNullException.ThrowIfNull(review);

        if (review.PlaceId != Id)
        {
            throw new DomainException("O depoimento do parceiro pertence a outro lugar.");
        }

        _partnerReviews.Add(review);
    }

    public void ApplyUserReview(int rating)
    {
        if (rating is < 1 or > 5)
        {
            throw new DomainException("A nota de avaliação deve ser entre 1 e 5.");
        }

        var totalPoints = (AverageRating * ReviewCount) + rating;
        ReviewCount++;
        AverageRating = Math.Round(totalPoints / ReviewCount, 2);
    }

    public void RecalculateRating(double newAverage, int totalReviews)
    {
        if (totalReviews < 0)
        {
            throw new DomainException("O total de avaliações não pode ser negativo.");
        }

        if (totalReviews == 0)
        {
            AverageRating = 0d;
            ReviewCount = 0;
            return;
        }

        if (newAverage is < 1.0 or > 5.0)
        {
            throw new DomainException("A média de avaliações deve estar entre 1.0 e 5.0.");
        }

        AverageRating = Math.Round(newAverage, 2);
        ReviewCount = totalReviews;
    }

    public void Approve()
    {
        if (Status == PublicationStatus.Rejected)
        {
            throw new DomainException("Um lugar rejeitado deve retornar para revisão antes de ser aprovado.");
        }

        Status = PublicationStatus.Approved;
    }

    public void Reject()
    {
        if (Status != PublicationStatus.UnderReview)
        {
            throw new DomainException("Apenas lugares em revisão podem ser rejeitados.");
        }

        Status = PublicationStatus.Rejected;
    }

    public void SendToReview() => Status = PublicationStatus.UnderReview;
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
