using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.ValueObjects;

/// <summary>
/// Represents an unregistered or temporary venue (e.g., "Mirantes I, II e III")
/// where street address and neighborhood are optional, but name and coordinates are required.
/// </summary>
public sealed record FreeLocation
{
    public string Name { get; }
    public string? AddressText { get; }
    public string? Neighborhood { get; }
    public GeoCoordinates Coordinates { get; }

    public FreeLocation(
        string name,
        string? addressText,
        string? neighborhood,
        GeoCoordinates coordinates)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("O nome do local livre é obrigatório.");
        }

        ArgumentNullException.ThrowIfNull(coordinates);

        Name = name.Trim();
        AddressText = string.IsNullOrWhiteSpace(addressText) ? null : addressText.Trim();
        Neighborhood = string.IsNullOrWhiteSpace(neighborhood) ? null : neighborhood.Trim();
        Coordinates = coordinates;
    }
}

/// <summary>
/// Public event organizer information (LGPD-compliant: does not store raw tax/personal IDs).
/// </summary>
public sealed record EventOrganizer
{
    public string Name { get; }
    public string? PublicContact { get; }
    public bool IsVerified { get; }

    public EventOrganizer(string name, string? publicContact = null, bool isVerified = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("O nome do organizador do evento é obrigatório.");
        }

        Name = name.Trim();
        PublicContact = string.IsNullOrWhiteSpace(publicContact) ? null : publicContact.Trim();
        IsVerified = isVerified;
    }
}

/// <summary>
/// Editorial verification metadata for a Place.
/// </summary>
public sealed record PlaceVerification
{
    public bool IsAddressVerified { get; }
    public bool IsScheduleVerified { get; }
    public bool AreCoordinatesVerified { get; }
    public IReadOnlyList<string> Sources { get; }
    public string? Notes { get; }

    public PlaceVerification(
        bool isAddressVerified,
        bool isScheduleVerified,
        bool areCoordinatesVerified,
        IEnumerable<string>? sources = null,
        string? notes = null)
    {
        IsAddressVerified = isAddressVerified;
        IsScheduleVerified = isScheduleVerified;
        AreCoordinatesVerified = areCoordinatesVerified;
        Sources = sources?
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim())
            .ToList()
            .AsReadOnly() ?? [];
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public static PlaceVerification Unverified() => new(false, false, false);
}
