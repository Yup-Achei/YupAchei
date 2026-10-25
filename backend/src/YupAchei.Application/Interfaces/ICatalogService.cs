using YupAchei.Application.DTOs;
using YupAchei.Domain.Entities;
using YupAchei.Domain.Repositories;

namespace YupAchei.Application.Interfaces;

public interface ICatalogService
{
    IReadOnlyList<CategoryDto> ListCategories(string? contentType = null);
    IReadOnlyList<TagDto> ListTags();
    Task<IReadOnlyList<PlaceDto>> ListPlacesAsync(PlaceSearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<PlaceDto?> GetPlaceByIdOrSlugAsync(string idOrSlug, DateTimeOffset? referenceTimestamp = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventDto>> ListActiveEventsAsync(EventSearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<EventDto?> GetEventByIdOrSlugAsync(string idOrSlug, CancellationToken cancellationToken = default);
}

public interface IRecommendationAgent
{
    Task<IReadOnlyList<Place>> SuggestPlacesAsync(
        string intent,
        IReadOnlyList<Place> activeCatalog,
        int maxSuggestions = 5,
        CancellationToken cancellationToken = default);
}

public interface IRecommendationService
{
    Task<RecommendationResponseDto> GetRecommendationsAsync(
        RecommendationRequestDto request,
        DateTimeOffset? referenceTimestamp = null,
        CancellationToken cancellationToken = default);
}
