using YupAchei.Application.Interfaces;
using YupAchei.Domain.Entities;

namespace YupAchei.Infrastructure.Services;

/// <summary>
/// Agente contextual de recomendação (Yupy AI) que analisa a intenção em texto livre do usuário
/// frente às descrições, categorias, etiquetas de clima e depoimentos de parceiros do catálogo ativo.
/// </summary>
public sealed class RecomendacaoAgenteLocalService : IRecomendacaoAgente
{
    public Task<IReadOnlyList<Lugar>> SugerirLugaresAsync(
        string intencao,
        IReadOnlyList<Lugar> catalogoAtivo,
        int maximoSugestoes = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(intencao) || catalogoAtivo.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<Lugar>>([]);
        }

        var palavrasChave = intencao
            .ToLowerInvariant()
            .Split([' ', ',', '.', '?', '!', ';', ':', '-'], StringSplitOptions.RemoveEmptyEntries)
            .Where(p => p.Length >= 3)
            .Distinct()
            .ToList();

        var ranqueados = catalogoAtivo
            .Select(lugar => new
            {
                Lugar = lugar,
                Pontuacao = CalcularPontuacaoContextual(lugar, palavrasChave)
            })
            .OrderByDescending(x => x.Pontuacao)
            .ThenByDescending(x => x.Lugar.NotaMedia)
            .Take(Math.Clamp(maximoSugestoes, 3, 5))
            .Select(x => x.Lugar)
            .ToList();

        return Task.FromResult<IReadOnlyList<Lugar>>(ranqueados);
    }

    private static double CalcularPontuacaoContextual(Lugar lugar, List<string> palavrasChave)
    {
        double pontos = lugar.NotaMedia;

        foreach (var palavra in palavrasChave)
        {
            if (lugar.Nome.Contains(palavra, StringComparison.OrdinalIgnoreCase))
            {
                pontos += 5.0;
            }

            if (lugar.Descricao.Contains(palavra, StringComparison.OrdinalIgnoreCase))
            {
                pontos += 3.0;
            }

            if (lugar.Categoria.ToString().Contains(palavra, StringComparison.OrdinalIgnoreCase))
            {
                pontos += 4.0;
            }

            if (lugar.EtiquetasClima.Any(t => t.Contains(palavra, StringComparison.OrdinalIgnoreCase)))
            {
                pontos += 4.0;
            }

            if (lugar.AvaliacoesParceiros.Any(p => p.ComentarioCurto.Contains(palavra, StringComparison.OrdinalIgnoreCase)))
            {
                pontos += 3.5;
            }
        }

        return pontos;
    }
}
