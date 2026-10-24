using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.ValueObjects;

public sealed record PriceRange
{
    public decimal ValorMinimo { get; }
    public decimal ValorMaximo { get; }
    public string Moeda { get; }
    public bool Gratuito => ValorMinimo == 0m && ValorMaximo == 0m;

    public PriceRange(decimal valorMinimo, decimal valorMaximo, string moeda = "BRL")
    {
        if (valorMinimo < 0m)
        {
            throw new DomainException("O valor mínimo da faixa de preço não pode ser negativo.");
        }

        if (valorMaximo < valorMinimo)
        {
            throw new DomainException("O valor máximo da faixa de preço não pode ser menor que o valor mínimo.");
        }

        if (string.IsNullOrWhiteSpace(moeda))
        {
            throw new DomainException("A moeda da faixa de preço é obrigatória.");
        }

        ValorMinimo = valorMinimo;
        ValorMaximo = valorMaximo;
        Moeda = moeda.Trim().ToUpperInvariant();
    }

    public static PriceRange Gratis(string moeda = "BRL") => new(0m, 0m, moeda);

    public static PriceRange APartirDe(decimal valorMinimo, string moeda = "BRL")
    {
        if (valorMinimo <= 0m)
        {
            throw new DomainException("Para eventos pagos 'a partir de', o valor mínimo deve ser maior que zero.");
        }

        return new PriceRange(valorMinimo, valorMinimo, moeda);
    }
}
