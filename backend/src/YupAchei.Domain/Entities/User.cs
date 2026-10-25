using YupAchei.Domain.Common;
using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.Entities;

public sealed class User : AggregateRoot
{
    private readonly HashSet<Guid> _savedPlaceIds = [];

    public string Name { get; private set; }
    public string Email { get; private set; }
    public IReadOnlySet<Guid> SavedPlaceIds => _savedPlaceIds;

    public User(string name, string email, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("O nome do usuário é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new DomainException("Um e-mail válido é obrigatório.");
        }

        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
    }

    public void SavePlace(Guid placeId)
    {
        if (placeId == Guid.Empty)
        {
            throw new DomainException("O identificador do lugar salvo não pode ser vazio.");
        }

        _savedPlaceIds.Add(placeId);
    }

    public void RemoveSavedPlace(Guid placeId)
    {
        _savedPlaceIds.Remove(placeId);
    }
}
