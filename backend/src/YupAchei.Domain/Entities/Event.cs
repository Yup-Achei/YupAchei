using YupAchei.Domain.Common;
using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;
using YupAchei.Domain.ValueObjects;

namespace YupAchei.Domain.Entities;

public sealed class Event : AggregateRoot
{
    private static readonly HashSet<PlaceCategory> ValidEventCategories =
    [
        PlaceCategory.Culture,
        PlaceCategory.Sports,
        PlaceCategory.Music,
        PlaceCategory.Parties,
        PlaceCategory.Gastronomy,
        PlaceCategory.Theater
    ];

    private readonly List<string> _vibeTags = [];
    private readonly List<MediaAsset> _photos = [];

    public string Slug { get; private set; }
    public bool IsSimulated { get; private set; }
    public string Title { get; private set; }
    public PlaceCategory Category { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public Guid? PlaceId { get; private set; }
    public FreeLocation? FreeLocation { get; private set; }
    public string? VenueName => FreeLocation?.Name;
    public PriceRange PriceRange { get; private set; }
    public string? TicketUrl { get; private set; }
    public EventOrganizer? Organizer { get; private set; }
    public bool IsFeatured { get; private set; }
    public bool IsSponsored { get; private set; }
    public PublicationStatus Status { get; private set; }
    public bool IsActive { get; private set; }

    public bool HasPlaceLink => PlaceId.HasValue;
    public bool HasFreeLocation => FreeLocation is not null;
    public IReadOnlyList<string> VibeTags => _vibeTags.AsReadOnly();
    public IReadOnlyList<MediaAsset> Photos => _photos.AsReadOnly();

    private Event(
        string title,
        string slug,
        PlaceCategory category,
        string description,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        Guid? placeId,
        FreeLocation? freeLocation,
        PriceRange priceRange,
        string? ticketUrl,
        EventOrganizer? organizer,
        IEnumerable<string>? vibeTags,
        IEnumerable<MediaAsset>? photos,
        bool isFeatured,
        bool isSponsored,
        bool isSimulated,
        Guid? id) : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("O título do evento é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("O slug do evento é obrigatório.");
        }

        if (!ValidEventCategories.Contains(category))
        {
            throw new DomainException($"A categoria '{category}' não é válida para um Evento.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("A descrição do evento é obrigatória.");
        }

        if (endDate <= startDate)
        {
            throw new DomainException("A data de término do evento deve ser posterior à data de início.");
        }

        var hasRegisteredPlace = placeId.HasValue && placeId.Value != Guid.Empty;
        var hasFreeLocation = freeLocation is not null;

        if (hasRegisteredPlace == hasFreeLocation)
        {
            throw new DomainException("O evento deve possuir exclusivamente um PlaceId cadastrado OU um FreeLocation.");
        }

        ArgumentNullException.ThrowIfNull(priceRange);

        Title = title.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        Category = category;
        Description = description.Trim();
        StartDate = startDate;
        EndDate = endDate;
        PlaceId = hasRegisteredPlace ? placeId : null;
        FreeLocation = freeLocation;
        PriceRange = priceRange;
        TicketUrl = string.IsNullOrWhiteSpace(ticketUrl) ? null : ticketUrl.Trim();
        Organizer = organizer;
        IsFeatured = isFeatured;
        IsSponsored = isSponsored;
        IsSimulated = isSimulated;
        Status = PublicationStatus.UnderReview;
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

    public static Event CreateForPlace(
        string title,
        string description,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        Guid placeId,
        PriceRange priceRange,
        string? ticketUrl = null,
        IEnumerable<string>? vibeTags = null,
        IEnumerable<MediaAsset>? photos = null,
        string? slug = null,
        PlaceCategory category = PlaceCategory.Culture,
        EventOrganizer? organizer = null,
        bool isFeatured = false,
        bool isSponsored = false,
        bool isSimulated = false,
        Guid? id = null)
    {
        if (placeId == Guid.Empty)
        {
            throw new DomainException("O identificador do Lugar vinculado deve ser válido.");
        }

        return new Event(
            title,
            slug ?? GenerateDefaultSlug(title),
            category,
            description,
            startDate,
            endDate,
            placeId,
            freeLocation: null,
            priceRange,
            ticketUrl,
            organizer,
            vibeTags,
            photos,
            isFeatured,
            isSponsored,
            isSimulated,
            id);
    }

    public static Event CreateForFreeLocation(
        string title,
        string description,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        FreeLocation freeLocation,
        PriceRange priceRange,
        string? ticketUrl = null,
        IEnumerable<string>? vibeTags = null,
        IEnumerable<MediaAsset>? photos = null,
        string? slug = null,
        PlaceCategory category = PlaceCategory.Culture,
        EventOrganizer? organizer = null,
        bool isFeatured = false,
        bool isSponsored = false,
        bool isSimulated = false,
        Guid? id = null)
    {
        ArgumentNullException.ThrowIfNull(freeLocation);

        return new Event(
            title,
            slug ?? GenerateDefaultSlug(title),
            category,
            description,
            startDate,
            endDate,
            placeId: null,
            freeLocation,
            priceRange,
            ticketUrl,
            organizer,
            vibeTags,
            photos,
            isFeatured,
            isSponsored,
            isSimulated,
            id);
    }

    public void Approve()
    {
        if (Status == PublicationStatus.Rejected)
        {
            throw new DomainException("Um evento rejeitado deve retornar para revisão antes de ser aprovado.");
        }

        Status = PublicationStatus.Approved;
    }

    public void Reject()
    {
        if (Status != PublicationStatus.UnderReview)
        {
            throw new DomainException("Apenas eventos em revisão podem ser rejeitados.");
        }

        Status = PublicationStatus.Rejected;
    }

    public void SendToReview() => Status = PublicationStatus.UnderReview;

    public bool IsActiveAt(DateTimeOffset timestamp) =>
        IsActive && Status == PublicationStatus.Approved && timestamp >= StartDate && timestamp <= EndDate;

    public bool IsAvailableInScheduleAt(DateTimeOffset now) =>
        IsActive && Status == PublicationStatus.Approved && EndDate >= now;

    private static string GenerateDefaultSlug(string title) =>
        string.IsNullOrWhiteSpace(title)
            ? string.Empty
            : string.Join('-', title.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));
}
