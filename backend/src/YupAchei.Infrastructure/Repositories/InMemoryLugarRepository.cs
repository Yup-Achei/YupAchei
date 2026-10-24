using System.Collections.Concurrent;
using YupAchei.Domain.Entities;
using YupAchei.Domain.Repositories;
using YupAchei.Infrastructure.Persistence;

namespace YupAchei.Infrastructure.Repositories;

public sealed class InMemoryLugarRepository : ILugarRepository
{
    private readonly ConcurrentDictionary<Guid, Lugar> _lugares;

    public InMemoryLugarRepository()
    {
        var iniciais = InMemoryCatalogoSeeder.CriarLugaresIniciais();
        _lugares = new ConcurrentDictionary<Guid, Lugar>(
            iniciais.ToDictionary(l => l.Id, l => l));
    }

    public Task<Lugar?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _lugares.TryGetValue(id, out var lugar);
        return Task.FromResult(lugar);
    }

    public Task<Lugar?> ObterPorSlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return Task.FromResult<Lugar?>(null);
        }

        var lugar = _lugares.Values.FirstOrDefault(l =>
            string.Equals(l.Slogan, slug.Trim(), StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(lugar);
    }

    public Task<IReadOnlyList<Lugar>> BuscarAsync(
        PlaceSearchCriteria criterios,
        CancellationToken cancellationToken = default)
    {
        var consulta = _lugares.Values.Where(l => l.EstaAprovado);

        if (!string.IsNullOrWhiteSpace(criterios.TermoBusca))
        {
            var termo = criterios.TermoBusca.Trim();
            consulta = consulta.Where(l =>
                l.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                l.Descricao.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                (l.Endereco.Bairro?.Contains(termo, StringComparison.OrdinalIgnoreCase) ?? false) ||
                l.EtiquetasClima.Any(t => t.Contains(termo, StringComparison.OrdinalIgnoreCase)));
        }

        if (criterios.Categoria.HasValue)
        {
            consulta = consulta.Where(l => l.Categoria == criterios.Categoria.Value);
        }

        if (criterios.Regiao.HasValue)
        {
            consulta = consulta.Where(l => l.Endereco.Regiao == criterios.Regiao.Value);
        }

        if (!string.IsNullOrWhiteSpace(criterios.Bairro))
        {
            var bairro = criterios.Bairro.Trim();
            consulta = consulta.Where(l =>
                l.Endereco.Bairro is not null &&
                l.Endereco.Bairro.Contains(bairro, StringComparison.OrdinalIgnoreCase));
        }

        if (criterios.EtiquetasClima is { Count: > 0 })
        {
            consulta = consulta.Where(l =>
                criterios.EtiquetasClima.All(tagFiltro =>
                    l.EtiquetasClima.Any(tagLugar =>
                        string.Equals(tagLugar, tagFiltro, StringComparison.OrdinalIgnoreCase))));
        }

        if (criterios.ApenasAbertoAgora)
        {
            var referencia = criterios.DataHoraReferencia ?? DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4));
            consulta = consulta.Where(l => l.EstaAbertoEm(referencia));
        }

        if (criterios.NotaMinima.HasValue)
        {
            consulta = consulta.Where(l => l.NotaMedia >= criterios.NotaMinima.Value);
        }

        var resultado = consulta
            .OrderByDescending(l => l.Patrocinado)
            .ThenByDescending(l => l.NotaMedia)
            .ThenBy(l => l.Nome)
            .ToList();

        return Task.FromResult<IReadOnlyList<Lugar>>(resultado);
    }

    public Task<IReadOnlyList<Lugar>> ObterFallbackCuradoriaAsync(
        int limiteResultados = 5,
        CancellationToken cancellationToken = default)
    {
        var curados = _lugares.Values
            .Where(l => l.EstaAprovado)
            .OrderByDescending(l => l.AvaliacoesParceiros.Count > 0)
            .ThenByDescending(l => l.NotaMedia)
            .ThenByDescending(l => l.QuantidadeAvaliacoes)
            .Take(Math.Max(1, limiteResultados))
            .ToList();

        return Task.FromResult<IReadOnlyList<Lugar>>(curados);
    }

    public Task AdicionarAsync(Lugar lugar, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lugar);
        _lugares[lugar.Id] = lugar;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Lugar lugar, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lugar);
        _lugares[lugar.Id] = lugar;
        return Task.CompletedTask;
    }
}
