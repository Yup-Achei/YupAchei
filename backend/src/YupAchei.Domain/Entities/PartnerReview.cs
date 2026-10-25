using YupAchei.Domain.Common;
using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.Entities;

public sealed class PartnerReview : Entity
{
    public const int MaxShortCommentLength = 200;

    public Guid PlaceId { get; }
    public Guid PartnerId { get; }
    public Partner? Partner { get; }
    public string BadgeOrScore { get; }
    public string ShortComment { get; }
    public bool IsFeatured { get; }

    public PartnerReview(
        Guid placeId,
        Guid partnerId,
        string badgeOrScore,
        string shortComment,
        bool isFeatured = true,
        Partner? partner = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (placeId == Guid.Empty)
        {
            throw new DomainException("O depoimento do parceiro deve estar vinculado a um Lugar válido.");
        }

        if (partnerId == Guid.Empty)
        {
            throw new DomainException("O depoimento deve referenciar um Parceiro válido.");
        }

        if (partner is not null && partner.Id != partnerId)
        {
            throw new DomainException("A instância de Partner informada não corresponde ao PartnerId.");
        }

        if (string.IsNullOrWhiteSpace(badgeOrScore))
        {
            throw new DomainException("O selo ou nota de indicação do parceiro é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(shortComment))
        {
            throw new DomainException("O comentário curto do parceiro é obrigatório.");
        }

        var normalizedComment = shortComment.Trim();
        if (normalizedComment.Length > MaxShortCommentLength)
        {
            throw new DomainException($"O comentário curto do parceiro não pode exceder {MaxShortCommentLength} caracteres.");
        }

        PlaceId = placeId;
        PartnerId = partnerId;
        Partner = partner;
        BadgeOrScore = badgeOrScore.Trim();
        ShortComment = normalizedComment;
        IsFeatured = isFeatured;
    }

    public static PartnerReview CreateWithPartner(
        Guid placeId,
        Partner partner,
        string badgeOrScore,
        string shortComment,
        bool isFeatured = true,
        Guid? id = null)
    {
        ArgumentNullException.ThrowIfNull(partner);
        return new PartnerReview(placeId, partner.Id, badgeOrScore, shortComment, isFeatured, partner, id);
    }
}
