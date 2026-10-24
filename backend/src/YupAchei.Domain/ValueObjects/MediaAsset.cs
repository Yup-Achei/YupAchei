using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.ValueObjects;

public sealed record MediaAsset
{
    public string Url { get; }
    public string? TextoAlternativo { get; }
    public bool EhCapa { get; }

    public MediaAsset(string url, string? textoAlternativo = null, bool ehCapa = false)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new DomainException("A URL da mídia é obrigatória.");
        }

        Url = url.Trim();
        TextoAlternativo = string.IsNullOrWhiteSpace(textoAlternativo) ? null : textoAlternativo.Trim();
        EhCapa = ehCapa;
    }
}
