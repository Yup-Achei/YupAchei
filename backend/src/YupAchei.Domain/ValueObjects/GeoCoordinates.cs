using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.ValueObjects;

public sealed record GeoCoordinates
{
    private const double EarthRadiusKm = 6371.0;

    public double Latitude { get; }
    public double Longitude { get; }

    public GeoCoordinates(double latitude, double longitude)
    {
        if (double.IsNaN(latitude) || latitude is < -90.0 or > 90.0)
        {
            throw new DomainException("Latitude deve estar entre -90 e 90 graus.");
        }

        if (double.IsNaN(longitude) || longitude is < -180.0 or > 180.0)
        {
            throw new DomainException("Longitude deve estar entre -180 e 180 graus.");
        }

        Latitude = latitude;
        Longitude = longitude;
    }

    public double DistanceInKilometersTo(GeoCoordinates other)
    {
        ArgumentNullException.ThrowIfNull(other);

        var dLat = DegreesToRadians(other.Latitude - Latitude);
        var dLon = DegreesToRadians(other.Longitude - Longitude);

        var lat1 = DegreesToRadians(Latitude);
        var lat2 = DegreesToRadians(other.Latitude);

        var a = Math.Pow(Math.Sin(dLat / 2), 2) +
                (Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon / 2), 2));

        var c = 2 * Math.Asin(Math.Sqrt(a));
        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * (Math.PI / 180.0);
}
