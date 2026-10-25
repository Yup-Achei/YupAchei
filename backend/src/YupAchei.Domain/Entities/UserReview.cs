using YupAchei.Domain.Common;
using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.Entities;

public sealed class UserReview : AggregateRoot
{
    public const int MinRating = 1;
    public const int MaxRating = 5;

    public Guid PlaceId { get; }
    public Guid UserId { get; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }
    public DateTimeOffset CreatedAt { get; }

    public UserReview(
        Guid placeId,
        Guid userId,
        int rating,
        string? comment = null,
        DateTimeOffset? createdAt = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (placeId == Guid.Empty)
        {
            throw new DomainException("A avaliação deve referenciar um Lugar válido.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainException("A avaliação deve referenciar um Usuário autenticado válido.");
        }

        if (rating is < MinRating or > MaxRating)
        {
            throw new DomainException($"A nota da avaliação deve estar entre {MinRating} e {MaxRating}.");
        }

        PlaceId = placeId;
        UserId = userId;
        Rating = rating;
        Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }
}
