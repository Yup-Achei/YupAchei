using YupAchei.Domain.Common;
using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.Entities;

public sealed class Parceiro : AggregateRoot
{
    public string Nome { get; private set; }
    public string UrlAvatar { get; private set; }
    public SocialNetwork RedeSocial { get; private set; }
    public string? UrlPerfilSocial { get; private set; }
    public bool Ativo { get; private set; }

    public Parceiro(
        string nome,
        string urlAvatar,
        SocialNetwork redeSocial,
        string? urlPerfilSocial = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome do parceiro/influenciador é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(urlAvatar))
        {
            throw new DomainException("A URL do avatar do parceiro é obrigatória.");
        }

        Nome = nome.Trim();
        UrlAvatar = urlAvatar.Trim();
        RedeSocial = redeSocial;
        UrlPerfilSocial = string.IsNullOrWhiteSpace(urlPerfilSocial) ? null : urlPerfilSocial.Trim();
        Ativo = true;
    }

    public void AtualizarPerfil(string nome, string urlAvatar, SocialNetwork redeSocial, string? urlPerfilSocial = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome do parceiro/influenciador é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(urlAvatar))
        {
            throw new DomainException("A URL do avatar do parceiro é obrigatória.");
        }

        Nome = nome.Trim();
        UrlAvatar = urlAvatar.Trim();
        RedeSocial = redeSocial;
        UrlPerfilSocial = string.IsNullOrWhiteSpace(urlPerfilSocial) ? null : urlPerfilSocial.Trim();
    }

    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
}
