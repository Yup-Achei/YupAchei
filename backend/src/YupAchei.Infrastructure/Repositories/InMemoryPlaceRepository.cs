using System.Collections.Concurrent;
using YupAchei.Domain.Entities;
using YupAchei.Domain.Repositories;
using YupAchei.Infrastructure.Persistence;

namespace YupAchei.Infrastructure.Repositories;

public sealed class InMemoryPlaceRepository : IPlaceRepository
{
    private readonly ConcurrentDictionary<Guid, Place> _places;

    public InMemoryPlaceRepository()
    {
        var initial = InMemoryCatalogSeeder.CreateInitialPlaces();
        _places = new ConcurrentDictionary<Guid, Place>(
            initial.ToDictionary(p => p.Id, p => p));
    }

    public Task<Place?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _places.TryGetValue(id, out var place);
        return Task.FromResult(place);
    }

    public Task<Place?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return Task.FromResult<Place?>(null);
        }

        var place = _places.Values.FirstOrDefault(p =>
            string.Equals(p.Slug, slug.Trim(), StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(place);
    }

    public Task<IReadOnlyList<Place>> SearchAsync(
        PlaceSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var query = _places.Values.Where(p => p.IsApproved);

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var term = criteria.SearchTerm.Trim();
            query = query.Where(p =>
                p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (p.Address.Neighborhood?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                p.VibeTags.Any(t => t.Contains(term, StringComparison.OrdinalIgnoreCase)));
        }

        if (criteria.Category.HasValue)
        {
            query = query.Where(p => p.Category == criteria.Category.Value);
        }

        if (criteria.Region.HasValue)
        {
            query = query.Where(p => p.Address.Region == criteria.Region.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Neighborhood))
        {
            var neighborhood = criteria.Neighborhood.Trim();
            query = query.Where(p =>
                p.Address.Neighborhood is not null &&
                p.Address.Neighborhood.Contains(neighborhood, StringComparison.OrdinalIgnoreCase));
        }

        if (criteria.VibeTags is { Count: > 0 })
        {
            query = query.Where(p =>
                criteria.VibeTags.All(filterTag =>
                    p.VibeTags.Any(placeTag =>
                        string.Equals(placeTag, filterTag, StringComparison.OrdinalIgnoreCase))));
        }

        if (criteria.OnlyOpenNow)
        {
            var reference = criteria.ReferenceTimestamp ?? DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4));
            query = query.Where(p => p.IsOpenAt(reference));
        }

        if (criteria.MinRating.HasValue)
        {
            query = query.Where(p => p.AverageRating >= criteria.MinRating.Value);
        }

        var result = query
            .OrderByDescending(p => p.IsSponsored)
            .ThenByDescending(p => p.AverageRating)
            .ThenBy(p => p.Name)
            .ToList();

        return Task.FromResult<IReadOnlyList<Place>>(result);
    }

    public Task<IReadOnlyList<Place>> GetCuratedFallbackAsync(
        int maxResults = 5,
        CancellationToken cancellationToken = default)
    {
        var curated = _places.Values
            .Where(p => p.IsApproved)
            .OrderByDescending(p => p.PartnerReviews.Count > 0)
            .ThenByDescending(p => p.AverageRating)
            .ThenByDescending(p => p.ReviewCount)
            .Take(Math.Max(1, maxResults))
            .ToList();

        return Task.FromResult<IReadOnlyList<Place>>(curated);
    }

    public Task AddAsync(Place place, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(place);
        _places[place.Id] = place;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Place place, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(place);
        _places[place.Id] = place;
        return Task.CompletedTask;
    }
}
