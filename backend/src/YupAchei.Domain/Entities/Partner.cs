using YupAchei.Domain.Common;
using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.Entities;

public sealed class Partner : AggregateRoot
{
    public string Name { get; private set; }
    public string AvatarUrl { get; private set; }
    public SocialNetwork SocialNetwork { get; private set; }
    public string? SocialProfileUrl { get; private set; }
    public bool IsActive { get; private set; }

    public Partner(
        string name,
        string avatarUrl,
        SocialNetwork socialNetwork,
        string? socialProfileUrl = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("O nome do parceiro/influenciador é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(avatarUrl))
        {
            throw new DomainException("A URL do avatar do parceiro é obrigatória.");
        }

        Name = name.Trim();
        AvatarUrl = avatarUrl.Trim();
        SocialNetwork = socialNetwork;
        SocialProfileUrl = string.IsNullOrWhiteSpace(socialProfileUrl) ? null : socialProfileUrl.Trim();
        IsActive = true;
    }

    public void UpdateProfile(string name, string avatarUrl, SocialNetwork socialNetwork, string? socialProfileUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("O nome do parceiro/influenciador é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(avatarUrl))
        {
            throw new DomainException("A URL do avatar do parceiro é obrigatória.");
        }

        Name = name.Trim();
        AvatarUrl = avatarUrl.Trim();
        SocialNetwork = socialNetwork;
        SocialProfileUrl = string.IsNullOrWhiteSpace(socialProfileUrl) ? null : socialProfileUrl.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
