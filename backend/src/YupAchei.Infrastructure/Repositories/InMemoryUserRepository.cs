using System.Collections.Concurrent;
using YupAchei.Domain.Entities;
using YupAchei.Domain.Repositories;

namespace YupAchei.Infrastructure.Repositories;

public sealed class InMemoryUserRepository : IUserRepository, IUserReviewRepository
{
    private readonly ConcurrentDictionary<Guid, User> _usuarios = new();
    private readonly ConcurrentDictionary<Guid, AvaliacaoUser> _avaliacoes = new();

    public Task<User?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _usuarios.TryGetValue(id, out var usuario);
        return Task.FromResult(usuario);
    }

    public Task<User?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Task.FromResult<User?>(null);
        }

        var usuario = _usuarios.Values.FirstOrDefault(u =>
            string.Equals(u.Email, email.Trim(), StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(usuario);
    }

    public Task AdicionarAsync(User usuario, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        _usuarios[usuario.Id] = usuario;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(User usuario, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        _usuarios[usuario.Id] = usuario;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AvaliacaoUser>> ObterPorLugarIdAsync(Guid lugarId, CancellationToken cancellationToken = default)
    {
        var lista = _avaliacoes.Values
            .Where(a => a.LugarId == lugarId)
            .OrderByDescending(a => a.CriadoEm)
            .ToList();

        return Task.FromResult<IReadOnlyList<AvaliacaoUser>>(lista);
    }

    public Task<AvaliacaoUser?> ObterPorLugarEUsuarioAsync(
        Guid lugarId,
        Guid usuarioId,
        CancellationToken cancellationToken = default)
    {
        var avaliacao = _avaliacoes.Values.FirstOrDefault(a =>
            a.LugarId == lugarId && a.UsuarioId == usuarioId);

        return Task.FromResult(avaliacao);
    }

    public Task AdicionarAsync(AvaliacaoUser avaliacao, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(avaliacao);
        _avaliacoes[avaliacao.Id] = avaliacao;
        return Task.CompletedTask;
    }
}
