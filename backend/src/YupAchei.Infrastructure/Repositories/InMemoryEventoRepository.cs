using System.Collections.Concurrent;
using YupAchei.Domain.Entities;
using YupAchei.Domain.Repositories;
using YupAchei.Infrastructure.Persistence;

namespace YupAchei.Infrastructure.Repositories;

public sealed class InMemoryEventoRepository : IEventoRepository
{
    private readonly ConcurrentDictionary<Guid, Evento> _eventos;

    public InMemoryEventoRepository()
    {
        var iniciais = InMemoryCatalogoSeeder.CriarEventosIniciais();
        _eventos = new ConcurrentDictionary<Guid, Evento>(
            iniciais.ToDictionary(e => e.Id, e => e));
    }

    public Task<Evento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _eventos.TryGetValue(id, out var evento);
        return Task.FromResult(evento);
    }

    public Task<IReadOnlyList<Evento>> BuscarAtivosAsync(
        EventSearchCriteria criterios,
        CancellationToken cancellationToken = default)
    {
        var agora = criterios.DataInicial ?? DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4));
        var consulta = _eventos.Values.Where(e => e.EstaDisponivelNaAgendaEm(agora));

        if (!string.IsNullOrWhiteSpace(criterios.TermoBusca))
        {
            var termo = criterios.TermoBusca.Trim();
            consulta = consulta.Where(e =>
                e.Titulo.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                e.Descricao.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                (e.NomeLocal?.Contains(termo, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (criterios.DataFinal.HasValue)
        {
            consulta = consulta.Where(e => e.DataInicio <= criterios.DataFinal.Value);
        }

        if (criterios.EtiquetasClima is { Count: > 0 })
        {
            consulta = consulta.Where(e =>
                criterios.EtiquetasClima.All(tagFiltro =>
                    e.EtiquetasClima.Any(tagEvento =>
                        string.Equals(tagEvento, tagFiltro, StringComparison.OrdinalIgnoreCase))));
        }

        if (criterios.ApenasGratuitos)
        {
            consulta = consulta.Where(e => e.FaixaPreco.Gratuito);
        }

        var resultado = consulta
            .OrderBy(e => e.DataInicio)
            .ToList();

        return Task.FromResult<IReadOnlyList<Evento>>(resultado);
    }

    public Task<IReadOnlyList<Evento>> ObterPorLugarIdAsync(
        Guid lugarId,
        CancellationToken cancellationToken = default)
    {
        var resultado = _eventos.Values
            .Where(e => e.LugarId == lugarId && e.Ativo)
            .OrderBy(e => e.DataInicio)
            .ToList();

        return Task.FromResult<IReadOnlyList<Evento>>(resultado);
    }

    public Task AdicionarAsync(Evento evento, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(evento);
        _eventos[evento.Id] = evento;
        return Task.CompletedTask;
    }
}
