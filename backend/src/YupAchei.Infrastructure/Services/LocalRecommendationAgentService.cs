using YupAchei.Application.Interfaces;
using YupAchei.Domain.Entities;

namespace YupAchei.Infrastructure.Services;

public sealed class LocalRecommendationAgentService : IRecommendationAgent
{
    public Task<IReadOnlyList<Place>> SuggestPlacesAsync(
        string intent,
        IReadOnlyList<Place> activeCatalog,
        int maxSuggestions = 5,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(intent) || activeCatalog.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<Place>>([]);
        }

        var keywords = intent
            .ToLowerInvariant()
            .Split([' ', ',', '.', '?', '!', ';', ':', '-'], StringSplitOptions.RemoveEmptyEntries)
            .Where(w => w.Length >= 3)
            .Distinct()
            .ToList();

        var ranked = activeCatalog
            .Select(place => new
            {
                Place = place,
                Score = CalculateContextualScore(place, keywords)
            })
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Place.AverageRating)
            .Take(Math.Clamp(maxSuggestions, 3, 5))
            .Select(x => x.Place)
            .ToList();

        return Task.FromResult<IReadOnlyList<Place>>(ranked);
    }

    private static double CalculateContextualScore(Place place, List<string> keywords)
    {
        double score = place.AverageRating;

        foreach (var word in keywords)
        {
            if (place.Name.Contains(word, StringComparison.OrdinalIgnoreCase))
            {
                score += 5.0;
            }

            if (place.Description.Contains(word, StringComparison.OrdinalIgnoreCase))
            {
                score += 3.0;
            }

            if (place.Category.ToString().Contains(word, StringComparison.OrdinalIgnoreCase))
            {
                score += 4.0;
            }

            if (place.VibeTags.Any(t => t.Contains(word, StringComparison.OrdinalIgnoreCase)))
            {
                score += 4.0;
            }

            if (place.PartnerReviews.Any(r => r.ShortComment.Contains(word, StringComparison.OrdinalIgnoreCase)))
            {
                score += 3.5;
            }
        }

        return score;
    }
}
