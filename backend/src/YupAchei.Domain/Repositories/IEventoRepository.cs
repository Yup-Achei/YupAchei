using YupAchei.Domain.Entities;
using YupAchei.Domain.Enums;

namespace YupAchei.Domain.Repositories;

public sealed record EventSearchCriteria(
    string? TermoBusca = null,
    UrbanRegion? Regiao = null,
    DateTimeOffset? DataInicial = null,
    DateTimeOffset? DataFinal = null,
    IReadOnlyList<string>? EtiquetasClima = null,
    bool ApenasGratuitos = false);

public interface IEventoRepository
{
    Task<Evento?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Evento>> BuscarAtivosAsync(EventSearchCriteria criterios, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Evento>> ObterPorLugarIdAsync(Guid lugarId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Evento evento, CancellationToken cancellationToken = default);
}
