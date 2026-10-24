using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.ValueObjects;

public sealed record Address
{
    public string? Logradouro { get; }
    public string Numero { get; }
    public string? Bairro { get; }
    public UrbanRegion? Regiao { get; }
    public string Cidade { get; }
    public string Estado { get; }
    public string? Cep { get; }
    public string? Complemento { get; }

    public Address(
        string? logradouro,
        string? numero,
        string? bairro,
        UrbanRegion? regiao,
        string cidade,
        string estado,
        string? cep = null,
        string? complemento = null)
    {
        if (string.IsNullOrWhiteSpace(cidade))
        {
            throw new DomainException("A cidade do endereço é obrigatória.");
        }

        if (string.IsNullOrWhiteSpace(estado) || estado.Trim().Length != 2)
        {
            throw new DomainException("A UF do estado deve conter exatamente 2 caracteres.");
        }

        Logradouro = string.IsNullOrWhiteSpace(logradouro) ? null : logradouro.Trim();
        Numero = string.IsNullOrWhiteSpace(numero) ? "s/n" : numero.Trim();
        Bairro = string.IsNullOrWhiteSpace(bairro) ? null : bairro.Trim();
        Regiao = regiao;
        Cidade = cidade.Trim();
        Estado = estado.Trim().ToUpperInvariant();
        Cep = string.IsNullOrWhiteSpace(cep) ? null : cep.Trim();
        Complemento = string.IsNullOrWhiteSpace(complemento) ? null : complemento.Trim();
    }
}
