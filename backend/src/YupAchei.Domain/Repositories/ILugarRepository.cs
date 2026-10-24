using YupAchei.Domain.Entities;
using YupAchei.Domain.Enums;

namespace YupAchei.Domain.Repositories;

public sealed record PlaceSearchCriteria(
    string? TermoBusca = null,
    CategoriaLugar? Categoria = null,
    UrbanRegion? Regiao = null,
    string? Bairro = null,
    IReadOnlyList<string>? EtiquetasClima = null,
    bool ApenasAbertoAgora = false,
    double? NotaMinima = null,
    DateTimeOffset? DataHoraReferencia = null);

public interface ILugarRepository
{
    Task<Lugar?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Lugar?> ObterPorSlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Lugar>> BuscarAsync(PlaceSearchCriteria criterios, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Lugar>> ObterFallbackCuradoriaAsync(int limiteResultados = 5, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Lugar lugar, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Lugar lugar, CancellationToken cancellationToken = default);
}
