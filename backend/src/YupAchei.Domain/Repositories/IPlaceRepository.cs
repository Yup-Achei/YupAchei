using YupAchei.Domain.Entities;
using YupAchei.Domain.Enums;

namespace YupAchei.Domain.Repositories;

public sealed record PlaceSearchCriteria(
    string? SearchTerm = null,
    PlaceCategory? Category = null,
    UrbanRegion? Region = null,
    string? Neighborhood = null,
    IReadOnlyList<string>? VibeTags = null,
    bool OnlyOpenNow = false,
    double? MinRating = null,
    DateTimeOffset? ReferenceTimestamp = null);

public interface IPlaceRepository
{
    Task<Place?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Place?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Place>> SearchAsync(PlaceSearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Place>> GetCuratedFallbackAsync(int maxResults = 5, CancellationToken cancellationToken = default);
    Task AddAsync(Place place, CancellationToken cancellationToken = default);
    Task UpdateAsync(Place place, CancellationToken cancellationToken = default);
}
