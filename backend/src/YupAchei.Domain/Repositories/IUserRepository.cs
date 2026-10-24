using YupAchei.Domain.Entities;

namespace YupAchei.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AdicionarAsync(User usuario, CancellationToken cancellationToken = default);
    Task AtualizarAsync(User usuario, CancellationToken cancellationToken = default);
}

public interface IUserReviewRepository
{
    Task<IReadOnlyList<AvaliacaoUser>> ObterPorLugarIdAsync(Guid lugarId, CancellationToken cancellationToken = default);
    Task<AvaliacaoUser?> ObterPorLugarEUsuarioAsync(Guid lugarId, Guid usuarioId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(AvaliacaoUser avaliacao, CancellationToken cancellationToken = default);
}
