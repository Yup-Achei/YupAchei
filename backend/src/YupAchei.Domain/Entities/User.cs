using YupAchei.Domain.Common;
using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.Entities;

public sealed class User : AggregateRoot
{
    private readonly HashSet<Guid> _lugaresSalvosIds = [];

    public string Nome { get; private set; }
    public string Email { get; private set; }
    public IReadOnlySet<Guid> LugaresSalvosIds => _lugaresSalvosIds;

    public User(string nome, string email, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome do usuário é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            throw new DomainException("Um e-mail válido é obrigatório.");
        }

        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
    }

    public void SalvarLugar(Guid lugarId)
    {
        if (lugarId == Guid.Empty)
        {
            throw new DomainException("O identificador do lugar salvo não pode ser vazio.");
        }

        _lugaresSalvosIds.Add(lugarId);
    }

    public void RemoverLugarSalvo(Guid lugarId)
    {
        _lugaresSalvosIds.Remove(lugarId);
    }
}
