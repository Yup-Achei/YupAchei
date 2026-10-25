using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.ValueObjects;

public sealed record Address
{
    public string? Street { get; }
    public string Number { get; }
    public string? Neighborhood { get; }
    public UrbanRegion? Region { get; }
    public string City { get; }
    public string State { get; }
    public string? ZipCode { get; }
    public string? Complement { get; }

    public Address(
        string? street,
        string? number,
        string? neighborhood,
        UrbanRegion? region,
        string city,
        string state,
        string? zipCode = null,
        string? complement = null)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new DomainException("A cidade do endereço é obrigatória.");
        }

        if (string.IsNullOrWhiteSpace(state) || state.Trim().Length != 2)
        {
            throw new DomainException("A UF do estado deve conter exatamente 2 caracteres.");
        }

        Street = string.IsNullOrWhiteSpace(street) ? null : street.Trim();
        Number = string.IsNullOrWhiteSpace(number) ? "s/n" : number.Trim();
        Neighborhood = string.IsNullOrWhiteSpace(neighborhood) ? null : neighborhood.Trim();
        Region = region;
        City = city.Trim();
        State = state.Trim().ToUpperInvariant();
        ZipCode = string.IsNullOrWhiteSpace(zipCode) ? null : zipCode.Trim();
        Complement = string.IsNullOrWhiteSpace(complement) ? null : complement.Trim();
    }
}
