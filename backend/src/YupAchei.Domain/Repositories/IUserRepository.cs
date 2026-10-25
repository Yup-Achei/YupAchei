using YupAchei.Domain.Entities;

namespace YupAchei.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
}

public interface IUserReviewRepository
{
    Task<IReadOnlyList<UserReview>> GetByPlaceIdAsync(Guid placeId, CancellationToken cancellationToken = default);
    Task<UserReview?> GetByPlaceAndUserAsync(Guid placeId, Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(UserReview review, CancellationToken cancellationToken = default);
}
