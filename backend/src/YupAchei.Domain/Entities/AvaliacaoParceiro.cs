using YupAchei.Domain.Common;
using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.Entities;

public sealed class AvaliacaoParceiro : Entity
{
    public const int TamanhoMaximoComentarioCurto = 200;

    public Guid LugarId { get; }
    public Guid ParceiroId { get; }
    public Parceiro? Parceiro { get; }
    public string SeloOuNota { get; }
    public string ComentarioCurto { get; }
    public bool EmDestaque { get; }

    public AvaliacaoParceiro(
        Guid lugarId,
        Guid parceiroId,
        string seloOuNota,
        string comentarioCurto,
        bool emDestaque = true,
        Parceiro? parceiro = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (lugarId == Guid.Empty)
        {
            throw new DomainException("O depoimento do parceiro deve estar vinculado a um Lugar válido.");
        }

        if (parceiroId == Guid.Empty)
        {
            throw new DomainException("O depoimento deve referenciar um Parceiro válido.");
        }

        if (parceiro is not null && parceiro.Id != parceiroId)
        {
            throw new DomainException("A instância de Parceiro informada não corresponde ao ParceiroId.");
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
        ParceiroId = parceiroId;
        Parceiro = parceiro;
        SeloOuNota = seloOuNota.Trim();
        ComentarioCurto = comentarioNormalizado;
        EmDestaque = emDestaque;
    }

    public static AvaliacaoParceiro CriarComParceiro(
        Guid lugarId,
        Parceiro parceiro,
        string seloOuNota,
        string comentarioCurto,
        bool emDestaque = true,
        Guid? id = null)
    {
        ArgumentNullException.ThrowIfNull(parceiro);
        return new AvaliacaoParceiro(lugarId, parceiro.Id, seloOuNota, comentarioCurto, emDestaque, parceiro, id);
    }
}
