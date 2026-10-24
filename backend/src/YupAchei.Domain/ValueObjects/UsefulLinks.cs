namespace YupAchei.Domain.ValueObjects;

public sealed record UsefulLinks
{
    public string? UrlGoogleMaps { get; }
    public string? PerfilInstagram { get; }
    public string? NumeroWhatsApp { get; }
    public string? Telefone { get; }
    public string? Email { get; }
    public string? UrlSite { get; }

    public UsefulLinks(
        string? urlGoogleMaps = null,
        string? perfilInstagram = null,
        string? numeroWhatsApp = null,
        string? urlSite = null,
        string? telefone = null,
        string? email = null)
    {
        UrlGoogleMaps = Normalizar(urlGoogleMaps);
        PerfilInstagram = Normalizar(perfilInstagram);
        NumeroWhatsApp = Normalizar(numeroWhatsApp);
        Telefone = Normalizar(telefone);
        Email = Normalizar(email);
        UrlSite = Normalizar(urlSite);
    }

    public static UsefulLinks Vazio() => new();

    private static string? Normalizar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
