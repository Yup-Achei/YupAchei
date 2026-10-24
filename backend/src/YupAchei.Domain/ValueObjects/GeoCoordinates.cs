using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.ValueObjects;

public sealed record GeoCoordinates
{
    private const double RaioTerraKm = 6371.0;

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

    public double CalcularDistanciaEmKmPara(GeoCoordinates destino)
    {
        ArgumentNullException.ThrowIfNull(destino);

        var diferencaLatitude = GrausParaRadianos(destino.Latitude - Latitude);
        var diferencaLongitude = GrausParaRadianos(destino.Longitude - Longitude);

        var latitudeOrigemRad = GrausParaRadianos(Latitude);
        var latitudeDestinoRad = GrausParaRadianos(destino.Latitude);

        var fatorA = Math.Pow(Math.Sin(diferencaLatitude / 2), 2) +
                     (Math.Cos(latitudeOrigemRad) * Math.Cos(latitudeDestinoRad) * Math.Pow(Math.Sin(diferencaLongitude / 2), 2));

        var fatorC = 2 * Math.Asin(Math.Sqrt(fatorA));
        return RaioTerraKm * fatorC;
    }

    private static double GrausParaRadianos(double graus) => graus * (Math.PI / 180.0);
}
