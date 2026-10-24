using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.ValueObjects;

/// <summary>
/// Representa um local avulso não cadastrado no catálogo fixo (ex.: Mirantes I, II e III),
/// onde o endereço formal e o bairro são opcionais, mas o nome e as coordenadas são obrigatórios.
/// </summary>
public sealed record LocalLivre
{
    public string Nome { get; }
    public string? Endereco { get; }
    public string? Bairro { get; }
    public GeoCoordinates Coordenadas { get; }

    public LocalLivre(
        string nome,
        string? endereco,
        string? bairro,
        GeoCoordinates coordenadas)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome do local livre é obrigatório.");
        }

        ArgumentNullException.ThrowIfNull(coordenadas);

        Nome = nome.Trim();
        Endereco = string.IsNullOrWhiteSpace(endereco) ? null : endereco.Trim();
        Bairro = string.IsNullOrWhiteSpace(bairro) ? null : bairro.Trim();
        Coordenadas = coordenadas;
    }
}

/// <summary>
/// Dados do organizador responsável por um evento (MEI ou CNPJ).
/// </summary>
public sealed record OrganizadorEvento
{
    public string Nome { get; }
    public TipoDocumentoOrganizador TipoDocumento { get; }
    public string Documento { get; }

    public OrganizadorEvento(string nome, TipoDocumentoOrganizador tipoDocumento, string documento)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome do organizador do evento é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(documento))
        {
            throw new DomainException("O documento do organizador do evento é obrigatório.");
        }

        Nome = nome.Trim();
        TipoDocumento = tipoDocumento;
        Documento = documento.Trim();
    }
}

/// <summary>
/// Metadados de verificação editorial de um Lugar (endereço, horário, coordenadas e fontes consultadas).
/// </summary>
public sealed record VerificacaoLugar
{
    public bool EnderecoVerificado { get; }
    public bool HorarioVerificado { get; }
    public bool CoordenadasVerificadas { get; }
    public IReadOnlyList<string> Fontes { get; }
    public string? Observacao { get; }

    public VerificacaoLugar(
        bool enderecoVerificado,
        bool horarioVerificado,
        bool coordenadasVerificadas,
        IEnumerable<string>? fontes = null,
        string? observacao = null)
    {
        EnderecoVerificado = enderecoVerificado;
        HorarioVerificado = horarioVerificado;
        CoordenadasVerificadas = coordenadasVerificadas;
        Fontes = fontes?
            .Where(f => !string.IsNullOrWhiteSpace(f))
            .Select(f => f.Trim())
            .ToList()
            .AsReadOnly() ?? [];
        Observacao = string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim();
    }

    public static VerificacaoLugar NaoVerificado() => new(false, false, false);
}
