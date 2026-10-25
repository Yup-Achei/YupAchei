using YupAchei.Domain.Entities;
using YupAchei.Domain.Enums;

namespace YupAchei.Domain.Repositories;

public sealed record EventSearchCriteria(
    string? SearchTerm = null,
    UrbanRegion? Region = null,
    DateTimeOffset? FromDate = null,
    DateTimeOffset? ToDate = null,
    IReadOnlyList<string>? VibeTags = null,
    bool OnlyFree = false);

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> SearchActiveAsync(EventSearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Event>> GetByPlaceIdAsync(Guid placeId, CancellationToken cancellationToken = default);
    Task AddAsync(Event @event, CancellationToken cancellationToken = default);
}
