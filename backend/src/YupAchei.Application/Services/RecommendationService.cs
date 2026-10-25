using YupAchei.Application.DTOs;
using YupAchei.Application.Interfaces;
using YupAchei.Domain.Exceptions;
using YupAchei.Domain.Repositories;

namespace YupAchei.Application.Services;

public sealed class RecommendationService : IRecommendationService
{
    private const int MinSuggestions = 3;
    private const int MaxSuggestions = 5;

    private readonly IPlaceRepository _placeRepository;
    private readonly IRecommendationAgent _recommendationAgent;

    public RecommendationService(
        IPlaceRepository placeRepository,
        IRecommendationAgent recommendationAgent)
    {
        _placeRepository = placeRepository;
        _recommendationAgent = recommendationAgent;
    }

    public async Task<RecommendationResponseDto> GetRecommendationsAsync(
        RecommendationRequestDto request,
        DateTimeOffset? referenceTimestamp = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Intent))
        {
            throw new DomainException("A intenção de busca para o Yupy AI não pode ser vazia.");
        }

        var normalizedIntent = request.Intent.Trim();
        var timestamp = referenceTimestamp ?? DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4));

        try
        {
            var activeCatalog = await _placeRepository.SearchAsync(new PlaceSearchCriteria(), cancellationToken);
            var agentSuggestions = await _recommendationAgent.SuggestPlacesAsync(
                normalizedIntent,
                activeCatalog,
                MaxSuggestions,
                cancellationToken);

            if (agentSuggestions.Count >= MinSuggestions)
            {
                var dtos = agentSuggestions
                    .Take(MaxSuggestions)
                    .Select(p => CatalogService.MapPlaceToDto(p, timestamp))
                    .ToList();

                return new RecommendationResponseDto(
                    Intent: normalizedIntent,
                    Suggestions: dtos,
                    UsedFallback: false,
                    Message: "Sugestões selecionadas pelo Yupy AI com base no que você procura.");
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            // RNF03: Fall back gracefully to curated static recommendations when the LLM fails or times out.
        }

        var fallbackPlaces = await _placeRepository.GetCuratedFallbackAsync(MaxSuggestions, cancellationToken);
        var fallbackDtos = fallbackPlaces
            .Take(MaxSuggestions)
            .Select(p => CatalogService.MapPlaceToDto(p, timestamp))
            .ToList();

        return new RecommendationResponseDto(
            Intent: normalizedIntent,
            Suggestions: fallbackDtos,
            UsedFallback: true,
            Message: "Confira os locais mais bem avaliados pela curadoria YupAchei em Porto Velho.");
    }
}
