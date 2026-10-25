using System.Collections.Concurrent;
using YupAchei.Domain.Entities;
using YupAchei.Domain.Repositories;

namespace YupAchei.Infrastructure.Repositories;

public sealed class InMemoryUserRepository : IUserRepository, IUserReviewRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();
    private readonly ConcurrentDictionary<Guid, UserReview> _reviews = new();

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Task.FromResult<User?>(null);
        }

        var user = _users.Values.FirstOrDefault(u =>
            string.Equals(u.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(user);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<UserReview>> GetByPlaceIdAsync(Guid placeId, CancellationToken cancellationToken = default)
    {
        var list = _reviews.Values
            .Where(r => r.PlaceId == placeId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<UserReview>>(list);
    }

    public Task<UserReview?> GetByPlaceAndUserAsync(
        Guid placeId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var review = _reviews.Values.FirstOrDefault(r =>
            r.PlaceId == placeId && r.UserId == userId);

        return Task.FromResult(review);
    }

    public Task AddAsync(UserReview review, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(review);
        _reviews[review.Id] = review;
        return Task.CompletedTask;
    }
}
