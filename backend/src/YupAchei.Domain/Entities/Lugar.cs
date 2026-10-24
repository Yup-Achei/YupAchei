using YupAchei.Domain.Common;
using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;
using YupAchei.Domain.ValueObjects;

namespace YupAchei.Domain.Entities;

public sealed class Lugar : AggregateRoot
{
    private static readonly HashSet<CategoriaLugar> CategoriasValidasLugar =
    [
        CategoriaLugar.Cafeterias,
        CategoriaLugar.Restaurantes,
        CategoriaLugar.Bares,
        CategoriaLugar.Cultura,
        CategoriaLugar.Passeios,
        CategoriaLugar.Esportes
    ];

    private readonly List<string> _etiquetasClima = [];
    private readonly List<MediaAsset> _fotos = [];
    private readonly List<AvaliacaoParceiro> _avaliacoesParceiros = [];

    public string Nome { get; private set; }
    public string Slogan { get; private set; }
    public string Descricao { get; private set; }
    public CategoriaLugar Categoria { get; private set; }
    public TipoLugar Tipo { get; private set; }
    public Address Endereco { get; private set; }
    public GeoCoordinates Coordenadas { get; private set; }
    public int FaixaPreco { get; private set; }
    public bool Aberto24h { get; private set; }
    public WeeklySchedule HorarioFuncionamento { get; private set; }
    public string? Observacoes { get; private set; }
    public UsefulLinks Links { get; private set; }
    public double NotaMedia { get; private set; }
    public int QuantidadeAvaliacoes { get; private set; }
    public string IndicadoPor { get; private set; }
    public bool Patrocinado { get; private set; }
    public StatusPublicacao Status { get; private set; }
    public VerificacaoLugar Verificacao { get; private set; }
    public DateTimeOffset AtualizadoEm { get; private set; }
    public bool Ativo { get; private set; }

    public bool EstaAprovado => Ativo && Status == StatusPublicacao.Aprovado;
    public IReadOnlyList<string> EtiquetasClima => _etiquetasClima.AsReadOnly();
    public IReadOnlyList<MediaAsset> Fotos => _fotos.AsReadOnly();
    public IReadOnlyList<AvaliacaoParceiro> AvaliacoesParceiros => _avaliacoesParceiros.AsReadOnly();

    public Lugar(
        string nome,
        string slogan,
        string descricao,
        CategoriaLugar categoria,
        Address endereco,
        GeoCoordinates coordenadas,
        WeeklySchedule? horarioFuncionamento = null,
        UsefulLinks? links = null,
        IEnumerable<string>? etiquetasClima = null,
        IEnumerable<MediaAsset>? fotos = null,
        TipoLugar tipo = TipoLugar.Comercial,
        int faixaPreco = 0,
        bool aberto24h = false,
        string? observacoes = null,
        string indicadoPor = "Equipe Yup Achei",
        bool patrocinado = false,
        StatusPublicacao status = StatusPublicacao.Aprovado,
        VerificacaoLugar? verificacao = null,
        DateTimeOffset? atualizadoEm = null,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O nome do lugar é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(slogan))
        {
            throw new DomainException("O slogan/slug do lugar é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new DomainException("A descrição do lugar é obrigatória.");
        }

        if (!CategoriasValidasLugar.Contains(categoria))
        {
            throw new DomainException($"A categoria '{categoria}' não é válida para um Lugar.");
        }

        if (faixaPreco is < 0 or > 4)
        {
            throw new DomainException("A faixa de preço do lugar deve estar entre 0 e 4.");
        }

        ArgumentNullException.ThrowIfNull(endereco);
        ArgumentNullException.ThrowIfNull(coordenadas);

        Nome = nome.Trim();
        Slogan = slogan.Trim();
        Descricao = descricao.Trim();
        Categoria = categoria;
        Tipo = tipo;
        Endereco = endereco;
        Coordenadas = coordenadas;
        FaixaPreco = faixaPreco;
        Aberto24h = aberto24h;
        HorarioFuncionamento = horarioFuncionamento ?? WeeklySchedule.Vazia();
        Observacoes = string.IsNullOrWhiteSpace(observacoes) ? null : observacoes.Trim();
        Links = links ?? UsefulLinks.Vazio();
        IndicadoPor = string.IsNullOrWhiteSpace(indicadoPor) ? "Equipe Yup Achei" : indicadoPor.Trim();
        Patrocinado = patrocinado;
        Status = status;
        Verificacao = verificacao ?? VerificacaoLugar.NaoVerificado();
        AtualizadoEm = atualizadoEm ?? DateTimeOffset.UtcNow;
        Ativo = true;

        if (etiquetasClima is not null)
        {
            foreach (var etiqueta in etiquetasClima.Where(t => !string.IsNullOrWhiteSpace(t)).Select(t => t.Trim()).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                _etiquetasClima.Add(etiqueta);
            }
        }

        if (fotos is not null)
        {
            _fotos.AddRange(fotos);
        }
    }

    public bool EstaAbertoEm(DateTimeOffset dataHora)
    {
        if (!EstaAprovado)
        {
            return false;
        }

        if (Aberto24h)
        {
            return true;
        }

        return HorarioFuncionamento.EstaAbertoEm(dataHora);
    }

    public void AdicionarAvaliacaoParceiro(AvaliacaoParceiro avaliacao)
    {
        ArgumentNullException.ThrowIfNull(avaliacao);

        if (avaliacao.LugarId != Id)
        {
            throw new DomainException("O depoimento do parceiro pertence a outro lugar.");
        }

        _avaliacoesParceiros.Add(avaliacao);
    }

    public void AplicarAvaliacaoUsuario(int nota)
    {
        if (nota is < 1 or > 5)
        {
            throw new DomainException("A nota de avaliação deve ser entre 1 e 5.");
        }

        var somaPontos = (NotaMedia * QuantidadeAvaliacoes) + nota;
        QuantidadeAvaliacoes++;
        NotaMedia = Math.Round(somaPontos / QuantidadeAvaliacoes, 2);
    }

    public void RecalcularNota(double novaMedia, int totalAvaliacoes)
    {
        if (totalAvaliacoes < 0)
        {
            throw new DomainException("O total de avaliações não pode ser negativo.");
        }

        if (totalAvaliacoes == 0)
        {
            NotaMedia = 0d;
            QuantidadeAvaliacoes = 0;
            return;
        }

        if (novaMedia is < 1.0 or > 5.0)
        {
            throw new DomainException("A média de avaliações deve estar entre 1.0 e 5.0.");
        }

        NotaMedia = Math.Round(novaMedia, 2);
        QuantidadeAvaliacoes = totalAvaliacoes;
    }

    public void Aprovar() => Status = StatusPublicacao.Aprovado;
    public void ColocarEmRevisao() => Status = StatusPublicacao.EmRevisao;
    public void Desativar() => Ativo = false;
    public void Ativar() => Ativo = true;
}
