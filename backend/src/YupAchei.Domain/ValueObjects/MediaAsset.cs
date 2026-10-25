using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.ValueObjects;

public sealed record MediaAsset
{
    public string Url { get; }
    public string? AltText { get; }
    public bool IsCover { get; }

    public MediaAsset(string url, string? altText = null, bool isCover = false)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new DomainException("A URL da mídia é obrigatória.");
        }

        Url = url.Trim();
        AltText = string.IsNullOrWhiteSpace(altText) ? null : altText.Trim();
        IsCover = isCover;
    }
}
