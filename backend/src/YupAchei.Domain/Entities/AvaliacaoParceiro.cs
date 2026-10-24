using YupAchei.Domain.Common;
using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.Entities;

public sealed class AvaliacaoParceiro : Entity
{
    public const int TamanhoMaximoComentarioCurto = 200;

    public Guid LugarId { get; }
    public string NomeParceiro { get; }
    public string UrlAvatar { get; }
    public SocialNetwork RedeSocial { get; }
    public string? UrlPerfilSocial { get; }
    public string SeloOuNota { get; }
    public string ComentarioCurto { get; }
    public bool EmDestaque { get; }

    public AvaliacaoParceiro(
        Guid lugarId,
        string nomeParceiro,
        string urlAvatar,
        SocialNetwork redeSocial,
        string seloOuNota,
        string comentarioCurto,
        bool emDestaque = true,
        string? urlPerfilSocial = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (lugarId == Guid.Empty)
        {
            throw new DomainException("O depoimento do parceiro deve estar vinculado a um Lugar válido.");
        }

        if (string.IsNullOrWhiteSpace(nomeParceiro))
        {
            throw new DomainException("O nome do parceiro/influenciador é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(urlAvatar))
        {
            throw new DomainException("A URL do avatar do parceiro é obrigatória.");
        }

        if (string.IsNullOrWhiteSpace(seloOuNota))
        {
            throw new DomainException("O selo ou nota de indicação do parceiro é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(comentarioCurto))
        {
            throw new DomainException("O comentário curto do parceiro é obrigatório.");
        }

        var comentarioNormalizado = comentarioCurto.Trim();
        if (comentarioNormalizado.Length > TamanhoMaximoComentarioCurto)
        {
            throw new DomainException($"O comentário curto do parceiro não pode exceder {TamanhoMaximoComentarioCurto} caracteres.");
        }

        LugarId = lugarId;
        NomeParceiro = nomeParceiro.Trim();
        UrlAvatar = urlAvatar.Trim();
        RedeSocial = redeSocial;
        UrlPerfilSocial = string.IsNullOrWhiteSpace(urlPerfilSocial) ? null : urlPerfilSocial.Trim();
        SeloOuNota = seloOuNota.Trim();
        ComentarioCurto = comentarioNormalizado;
        EmDestaque = emDestaque;
    }
}
