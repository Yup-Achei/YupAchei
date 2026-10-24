using YupAchei.Application.DTOs;
using YupAchei.Domain.Entities;
using YupAchei.Domain.Repositories;

namespace YupAchei.Application.Interfaces;

public interface ICatalogoService
{
    IReadOnlyList<CategoriaDto> ListarCategorias(string? tipo = null);
    IReadOnlyList<TagDto> ListarTags();
    Task<IReadOnlyList<LugarDto>> ListarLugaresAsync(PlaceSearchCriteria criterios, CancellationToken cancellationToken = default);
    Task<LugarDto?> ObterLugarPorIdOuSlugAsync(string idOuSlug, DateTimeOffset? referencia = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventoDto>> ListarEventosAtivosAsync(EventSearchCriteria criterios, CancellationToken cancellationToken = default);
    Task<EventoDto?> ObterEventoPorIdOuSlugAsync(string idOuSlug, CancellationToken cancellationToken = default);
}

public interface IRecomendacaoAgente
{
    Task<IReadOnlyList<Lugar>> SugerirLugaresAsync(
        string intencao,
        IReadOnlyList<Lugar> catalogoAtivo,
        int maximoSugestoes = 5,
        CancellationToken cancellationToken = default);
}

public interface IRecomendacaoService
{
    Task<RecomendacaoResponseDto> ObterRecomendacoesAsync(
        RecomendacaoRequestDto request,
        DateTimeOffset? referencia = null,
        CancellationToken cancellationToken = default);
}
