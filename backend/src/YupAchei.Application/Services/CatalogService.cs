using YupAchei.Application.DTOs;
using YupAchei.Application.Interfaces;
using YupAchei.Domain.Entities;
using YupAchei.Domain.Enums;
using YupAchei.Domain.Repositories;

namespace YupAchei.Application.Services;

public sealed class CatalogService : ICatalogService
{
    private static readonly IReadOnlyList<CategoryDto> DefaultCategories =
    [
        new("cafeterias", "Cafeterias", ["local"], "coffee", "#F59E0B", 1),
        new("restaurantes", "Restaurantes", ["local"], "utensils", "#F97316", 2),
        new("bares", "Bares", ["local"], "beer", "#8B5CF6", 3),
        new("cultura", "Cultura", ["local", "evento"], "palette", "#3B82F6", 4),
        new("passeios", "Passeios", ["local"], "compass", "#10B981", 5),
        new("esportes", "Esportes", ["local", "evento"], "dumbbell", "#EF4444", 6),
        new("musica", "Música", ["evento"], "music", "#EC4899", 7),
        new("festas", "Festas", ["evento"], "party-popper", "#A855F7", 8),
        new("gastronomia", "Gastronomia", ["evento"], "chef-hat", "#F97316", 9),
        new("teatro", "Teatro", ["evento"], "drama", "#6366F1", 10)
    ];

    private static readonly IReadOnlyList<TagDto> DefaultTags =
    [
        new("gratuito", "Gratuito", "tag"),
        new("familia", "Bom pra família", "baby"),
        new("pet-friendly", "Pet friendly", "dog"),
        new("sem-pets", "Não aceita pets", "ban"),
        new("acessivel", "Acessível", "accessibility"),
        new("estacionamento", "Estacionamento", "car"),
        new("wifi", "Wi-Fi", "wifi"),
        new("musica-ao-vivo", "Música ao vivo", "mic"),
        new("ar-livre", "Ao ar livre", "trees"),
        new("por-do-sol", "Pôr do sol", "sunset"),
        new("vista-rio", "Vista pro rio", "waves"),
        new("comida-regional", "Comida regional", "fish"),
        new("vegano", "Opções veganas", "leaf"),
        new("trabalhar", "Bom pra trabalhar", "laptop"),
        new("historico", "Histórico", "landmark"),
        new("climatizado", "Climatizado", "snowflake")
    ];

    private readonly IPlaceRepository _placeRepository;
    private readonly IEventRepository _eventRepository;

    public CatalogService(IPlaceRepository placeRepository, IEventRepository eventRepository)
    {
        _placeRepository = placeRepository;
        _eventRepository = eventRepository;
    }

    public IReadOnlyList<CategoryDto> ListCategories(string? contentType = null)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return DefaultCategories;
        }

        var normalizedType = contentType.Trim().ToLowerInvariant();
        return DefaultCategories
            .Where(c => c.Types.Contains(normalizedType, StringComparer.OrdinalIgnoreCase))
            .OrderBy(c => c.Order)
            .ToList();
    }

    public IReadOnlyList<TagDto> ListTags() => DefaultTags;

    public async Task<IReadOnlyList<PlaceDto>> ListPlacesAsync(
        PlaceSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var referenceTimestamp = criteria.ReferenceTimestamp ?? DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4));
        var places = await _placeRepository.SearchAsync(criteria, cancellationToken);

        return places
            .Where(p => p.IsApproved)
            .Select(p => MapPlaceToDto(p, referenceTimestamp))
            .ToList();
    }

    public async Task<PlaceDto?> GetPlaceByIdOrSlugAsync(
        string idOrSlug,
        DateTimeOffset? referenceTimestamp = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idOrSlug))
        {
            return null;
        }

        Place? place = null;
        if (Guid.TryParse(idOrSlug, out var id))
        {
            place = await _placeRepository.GetByIdAsync(id, cancellationToken);
        }

        place ??= await _placeRepository.GetBySlugAsync(idOrSlug.Trim().ToLowerInvariant(), cancellationToken);

        if (place is null || !place.IsApproved)
        {
            return null;
        }

        var timestamp = referenceTimestamp ?? DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4));
        return MapPlaceToDto(place, timestamp);
    }

    public async Task<IReadOnlyList<EventDto>> ListActiveEventsAsync(
        EventSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var events = await _eventRepository.SearchActiveAsync(criteria, cancellationToken);
        return events
            .OrderBy(e => e.StartDate)
            .Select(MapEventToDto)
            .ToList();
    }

    public async Task<EventDto?> GetEventByIdOrSlugAsync(
        string idOrSlug,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idOrSlug))
        {
            return null;
        }

        if (Guid.TryParse(idOrSlug, out var id))
        {
            var byId = await _eventRepository.GetByIdAsync(id, cancellationToken);
            if (byId is not null)
            {
                return MapEventToDto(byId);
            }
        }

        var activeEvents = await _eventRepository.SearchActiveAsync(new EventSearchCriteria(), cancellationToken);
        var bySlug = activeEvents.FirstOrDefault(e =>
            string.Equals(e.Slug, idOrSlug.Trim(), StringComparison.OrdinalIgnoreCase) ||
            string.Equals(e.Id.ToString(), idOrSlug.Trim(), StringComparison.OrdinalIgnoreCase));

        return bySlug is null ? null : MapEventToDto(bySlug);
    }

    internal static PlaceDto MapPlaceToDto(Place place, DateTimeOffset referenceTimestamp)
    {
        var mainPhoto = place.Photos.FirstOrDefault(p => p.IsCover)?.Url
                        ?? place.Photos.FirstOrDefault()?.Url
                        ?? "https://picsum.photos/seed/yupachei/800/600";

        var gallery = place.Photos.Where(p => !p.IsCover).Select(p => p.Url).ToList();

        return new PlaceDto(
            Id: place.Id.ToString(),
            Slug: place.Slug,
            Name: place.Name,
            CategoryId: MapCategoryToSlug(place.Category),
            Kind: place.Kind == PlaceKind.Public ? "publico" : "comercial",
            Description: place.Description,
            Address: new AddressDto(
                place.Address.Street,
                place.Address.Number,
                place.Address.Neighborhood,
                place.Address.City,
                place.Address.State,
                place.Address.ZipCode),
            Coordinates: new CoordinatesDto(place.Coordinates.Latitude, place.Coordinates.Longitude),
            PriceRange: new PriceRangeDto(
                place.PriceRange.IsFree,
                place.PriceRange.MinAmount,
                place.PriceRange.MaxAmount,
                place.PriceRange.Currency),
            IsOpen24Hours: place.IsOpen24Hours,
            IsOpenNow: place.IsOpenAt(referenceTimestamp),
            OpeningHours: place.OpeningHours.Entries
                .Select(h => new OpeningHoursDto(
                    MapWeekday(h.DayOfWeek),
                    h.OpenTime.ToString("HH:mm"),
                    h.CloseTime.ToString("HH:mm")))
                .ToList(),
            Notes: place.Notes,
            Tags: place.VibeTags,
            Contact: new ContactDto(
                place.Links.InstagramHandle,
                place.Links.WhatsAppNumber,
                place.Links.Phone,
                place.Links.Email,
                place.Links.WebsiteUrl),
            Images: new PlaceImagesDto(mainPhoto, gallery),
            Rating: new RatingSummaryDto(place.AverageRating, place.ReviewCount),
            PartnerReviews: place.PartnerReviews
                .Select(r => new PartnerReviewDto(
                    r.Id,
                    r.PartnerId,
                    r.Partner?.Name,
                    r.Partner?.AvatarUrl,
                    r.Partner?.SocialNetwork.ToString().ToLowerInvariant(),
                    r.Partner?.SocialProfileUrl,
                    r.BadgeOrScore,
                    r.ShortComment,
                    r.IsFeatured))
                .ToList(),
            RecommendedBy: place.RecommendedBy,
            IsSponsored: place.IsSponsored,
            Status: MapStatus(place.Status),
            Verification: new VerificationDto(
                place.Verification.IsAddressVerified,
                place.Verification.IsScheduleVerified,
                place.Verification.AreCoordinatesVerified,
                place.Verification.Sources,
                place.Verification.Notes),
            UpdatedAt: place.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:sszzz"));
    }

    internal static EventDto MapEventToDto(Event @event)
    {
        var image = @event.Photos.FirstOrDefault()?.Url ?? "https://picsum.photos/seed/evento/800/600";

        return new EventDto(
            Id: @event.Id.ToString(),
            Slug: @event.Slug,
            IsSimulated: @event.IsSimulated,
            Title: @event.Title,
            CategoryId: MapCategoryToSlug(@event.Category),
            Description: @event.Description,
            StartDate: @event.StartDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
            EndDate: @event.EndDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
            PlaceId: @event.PlaceId?.ToString(),
            FreeLocation: @event.FreeLocation is null
                ? null
                : new FreeLocationDto(
                    @event.FreeLocation.Name,
                    @event.FreeLocation.AddressText,
                    @event.FreeLocation.Neighborhood,
                    new CoordinatesDto(@event.FreeLocation.Coordinates.Latitude, @event.FreeLocation.Coordinates.Longitude)),
            PriceRange: new PriceRangeDto(
                @event.PriceRange.IsFree,
                @event.PriceRange.MinAmount,
                @event.PriceRange.MaxAmount,
                @event.PriceRange.Currency),
            TicketUrl: @event.TicketUrl,
            Image: image,
            Organizer: @event.Organizer is null
                ? null
                : new OrganizerDto(
                    @event.Organizer.Name,
                    @event.Organizer.PublicContact,
                    @event.Organizer.IsVerified),
            Tags: @event.VibeTags,
            IsFeatured: @event.IsFeatured,
            IsSponsored: @event.IsSponsored,
            Status: MapStatus(@event.Status));
    }

    public static string MapCategoryToSlug(PlaceCategory category) => category switch
    {
        PlaceCategory.Cafes => "cafeterias",
        PlaceCategory.Restaurants => "restaurantes",
        PlaceCategory.Bars => "bares",
        PlaceCategory.Culture => "cultura",
        PlaceCategory.Tours => "passeios",
        PlaceCategory.Sports => "esportes",
        PlaceCategory.Music => "musica",
        PlaceCategory.Parties => "festas",
        PlaceCategory.Gastronomy => "gastronomia",
        PlaceCategory.Theater => "teatro",
        _ => "passeios"
    };

    public static PlaceCategory? TryParseCategory(string? slug) =>
        slug?.Trim().ToLowerInvariant() switch
        {
            "cafeterias" or "cafes" => PlaceCategory.Cafes,
            "restaurantes" or "restaurants" => PlaceCategory.Restaurants,
            "bares" or "bars" => PlaceCategory.Bars,
            "cultura" or "culture" => PlaceCategory.Culture,
            "passeios" or "tours" => PlaceCategory.Tours,
            "esportes" or "sports" => PlaceCategory.Sports,
            "musica" or "música" or "music" => PlaceCategory.Music,
            "festas" or "parties" => PlaceCategory.Parties,
            "gastronomia" or "gastronomy" => PlaceCategory.Gastronomy,
            "teatro" or "theater" => PlaceCategory.Theater,
            _ => null
        };

    private static string MapStatus(PublicationStatus status) => status switch
    {
        PublicationStatus.Approved => "aprovado",
        PublicationStatus.Rejected => "rejeitado",
        _ => "em_revisao"
    };

    private static string MapWeekday(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => "seg",
        DayOfWeek.Tuesday => "ter",
        DayOfWeek.Wednesday => "qua",
        DayOfWeek.Thursday => "qui",
        DayOfWeek.Friday => "sex",
        DayOfWeek.Saturday => "sab",
        DayOfWeek.Sunday => "dom",
        _ => "seg"
    };
}
