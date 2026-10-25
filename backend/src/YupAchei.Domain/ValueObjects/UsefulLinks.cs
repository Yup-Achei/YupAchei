namespace YupAchei.Domain.ValueObjects;

public sealed record UsefulLinks
{
    public string? GoogleMapsUrl { get; }
    public string? InstagramHandle { get; }
    public string? WhatsAppNumber { get; }
    public string? Phone { get; }
    public string? Email { get; }
    public string? WebsiteUrl { get; }

    public UsefulLinks(
        string? googleMapsUrl = null,
        string? instagramHandle = null,
        string? whatsAppNumber = null,
        string? websiteUrl = null,
        string? phone = null,
        string? email = null)
    {
        GoogleMapsUrl = Normalize(googleMapsUrl);
        InstagramHandle = Normalize(instagramHandle);
        WhatsAppNumber = Normalize(whatsAppNumber);
        Phone = Normalize(phone);
        Email = Normalize(email);
        WebsiteUrl = Normalize(websiteUrl);
    }

    public static UsefulLinks Empty() => new();

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
