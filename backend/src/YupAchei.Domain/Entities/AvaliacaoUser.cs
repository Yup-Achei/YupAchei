using YupAchei.Domain.Common;
using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.Entities;

public sealed class AvaliacaoUser : AggregateRoot
{
    public const int NotaMinima = 1;
    public const int NotaMaxima = 5;

    public Guid LugarId { get; }
    public Guid UsuarioId { get; }
    public int Nota { get; private set; }
    public string? Comentario { get; private set; }
    public DateTimeOffset CriadoEm { get; }

    public AvaliacaoUser(
        Guid lugarId,
        Guid usuarioId,
        int nota,
        string? comentario = null,
        DateTimeOffset? criadoEm = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (lugarId == Guid.Empty)
        {
            throw new DomainException("A avaliação deve referenciar um Lugar válido.");
        }

        if (usuarioId == Guid.Empty)
        {
            throw new DomainException("A avaliação deve referenciar um Usuário autenticado válido.");
        }

        if (nota is < NotaMinima or > NotaMaxima)
        {
            throw new DomainException($"A nota da avaliação deve estar entre {NotaMinima} e {NotaMaxima}.");
        }

        LugarId = lugarId;
        UsuarioId = usuarioId;
        Nota = nota;
        Comentario = string.IsNullOrWhiteSpace(comentario) ? null : comentario.Trim();
        CriadoEm = criadoEm ?? DateTimeOffset.UtcNow;
    }
}
