using YupAchei.Application.DTOs;
using YupAchei.Application.Interfaces;
using YupAchei.Domain.Exceptions;
using YupAchei.Domain.Repositories;

namespace YupAchei.Application.Services;

public sealed class RecomendacaoService : IRecomendacaoService
{
    private const int MinSugestoes = 3;
    private const int MaxSugestoes = 5;

    private readonly ILugarRepository _lugarRepository;
    private readonly IRecomendacaoAgente _recomendacaoAgente;

    public RecomendacaoService(
        ILugarRepository lugarRepository,
        IRecomendacaoAgente recomendacaoAgente)
    {
        _lugarRepository = lugarRepository;
        _recomendacaoAgente = recomendacaoAgente;
    }

    public async Task<RecomendacaoResponseDto> ObterRecomendacoesAsync(
        RecomendacaoRequestDto request,
        DateTimeOffset? referencia = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Intencao))
        {
            throw new DomainException("A intenção de busca para o Yupy AI não pode ser vazia.");
        }

        var intencaoNormalizada = request.Intencao.Trim();
        var dataReferencia = referencia ?? DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4));

        try
        {
            var catalogoAtivo = await _lugarRepository.BuscarAsync(new PlaceSearchCriteria(), cancellationToken);
            var sugestoesAgente = await _recomendacaoAgente.SugerirLugaresAsync(
                intencaoNormalizada,
                catalogoAtivo,
                MaxSugestoes,
                cancellationToken);

            if (sugestoesAgente.Count >= MinSugestoes)
            {
                var dtos = sugestoesAgente
                    .Take(MaxSugestoes)
                    .Select(l => CatalogoService.MapearLugarParaDto(l, dataReferencia))
                    .ToList();

                return new RecomendacaoResponseDto(
                    Intencao: intencaoNormalizada,
                    Sugestoes: dtos,
                    UsouFallback: false,
                    Mensagem: "Sugestões selecionadas pelo Yupy AI com base no que você procura.");
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            // RNF03: Em caso de erro, timeout ou indisponibilidade da LLM, aciona o fallback estático sem quebrar a tela.
        }

        var fallbackLugares = await _lugarRepository.ObterFallbackCuradoriaAsync(MaxSugestoes, cancellationToken);
        var fallbackDtos = fallbackLugares
            .Take(MaxSugestoes)
            .Select(l => CatalogoService.MapearLugarParaDto(l, dataReferencia))
            .ToList();

        return new RecomendacaoResponseDto(
            Intencao: intencaoNormalizada,
            Sugestoes: fallbackDtos,
            UsouFallback: true,
            Mensagem: "Confira os locais mais bem avaliados pela curadoria YupAchei em Porto Velho.");
    }
}
