using System.Collections.Concurrent;
using YupAchei.Domain.Entities;
using YupAchei.Domain.Repositories;
using YupAchei.Infrastructure.Persistence;

namespace YupAchei.Infrastructure.Repositories;

public sealed class InMemoryEventRepository : IEventRepository
{
    private readonly ConcurrentDictionary<Guid, Event> _events;

    public InMemoryEventRepository()
    {
        var initial = InMemoryCatalogSeeder.CreateInitialEvents();
        _events = new ConcurrentDictionary<Guid, Event>(
            initial.ToDictionary(e => e.Id, e => e));
    }

    public Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _events.TryGetValue(id, out var @event);
        return Task.FromResult(@event);
    }

    public Task<IReadOnlyList<Event>> SearchActiveAsync(
        EventSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var now = criteria.FromDate ?? DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4));
        var query = _events.Values.Where(e => e.IsAvailableInScheduleAt(now));

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var term = criteria.SearchTerm.Trim();
            query = query.Where(e =>
                e.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                e.Description.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (e.VenueName?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (criteria.ToDate.HasValue)
        {
            query = query.Where(e => e.StartDate <= criteria.ToDate.Value);
        }

        if (criteria.VibeTags is { Count: > 0 })
        {
            query = query.Where(e =>
                criteria.VibeTags.All(filterTag =>
                    e.VibeTags.Any(eventTag =>
                        string.Equals(eventTag, filterTag, StringComparison.OrdinalIgnoreCase))));
        }

        if (criteria.OnlyFree)
        {
            query = query.Where(e => e.PriceRange.IsFree);
        }

        var result = query
            .OrderBy(e => e.StartDate)
            .ToList();

        return Task.FromResult<IReadOnlyList<Event>>(result);
    }

    public Task<IReadOnlyList<Event>> GetByPlaceIdAsync(
        Guid placeId,
        CancellationToken cancellationToken = default)
    {
        var result = _events.Values
            .Where(e => e.PlaceId == placeId && e.IsActive)
            .OrderBy(e => e.StartDate)
            .ToList();

        return Task.FromResult<IReadOnlyList<Event>>(result);
    }

    public Task AddAsync(Event @event, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(@event);
        _events[@event.Id] = @event;
        return Task.CompletedTask;
    }
}
