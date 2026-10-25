using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.ValueObjects;

public sealed record PriceRange
{
    public decimal MinAmount { get; }
    public decimal MaxAmount { get; }
    public string Currency { get; }
    public bool IsFree => MinAmount == 0m && MaxAmount == 0m;

    public PriceRange(decimal minAmount, decimal maxAmount, string currency = "BRL")
    {
        if (minAmount < 0m)
        {
            throw new DomainException("O valor mínimo da faixa de preço não pode ser negativo.");
        }

        if (maxAmount < minAmount)
        {
            throw new DomainException("O valor máximo da faixa de preço não pode ser menor que o valor mínimo.");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException("A moeda da faixa de preço é obrigatória.");
        }

        MinAmount = minAmount;
        MaxAmount = maxAmount;
        Currency = currency.Trim().ToUpperInvariant();
    }

    public static PriceRange Free(string currency = "BRL") => new(0m, 0m, currency);

    public static PriceRange StartingFrom(decimal minAmount, string currency = "BRL")
    {
        if (minAmount <= 0m)
        {
            throw new DomainException("Para valores 'a partir de', o valor mínimo deve ser maior que zero.");
        }

        return new PriceRange(minAmount, minAmount, currency);
    }
}
