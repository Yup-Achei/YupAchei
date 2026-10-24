using YupAchei.Domain.Common;
using YupAchei.Domain.Enums;
using YupAchei.Domain.Exceptions;
using YupAchei.Domain.ValueObjects;

namespace YupAchei.Domain.Entities;

public sealed class Evento : AggregateRoot
{
    private static readonly HashSet<CategoriaLugar> CategoriasValidasEvento =
    [
        CategoriaLugar.Cultura,
        CategoriaLugar.Esportes,
        CategoriaLugar.Musica,
        CategoriaLugar.Festas,
        CategoriaLugar.Gastronomia,
        CategoriaLugar.Teatro
    ];

    private readonly List<string> _etiquetasClima = [];
    private readonly List<MediaAsset> _fotos = [];

    public string Slug { get; private set; }
    public bool Simulado { get; private set; }
    public string Titulo { get; private set; }
    public CategoriaLugar Categoria { get; private set; }
    public string Descricao { get; private set; }
    public DateTimeOffset DataInicio { get; private set; }
    public DateTimeOffset DataFim { get; private set; }
    public Guid? LugarId { get; private set; }
    public LocalLivre? LocalLivre { get; private set; }
    public string? NomeLocal => LocalLivre?.Nome;
    public PriceRange FaixaPreco { get; private set; }
    public string? TicketUrl { get; private set; }
    public OrganizadorEvento? Organizador { get; private set; }
    public bool Destaque { get; private set; }
    public bool Patrocinado { get; private set; }
    public StatusPublicacao Status { get; private set; }
    public bool Ativo { get; private set; }

    public bool PossuiVinculoLugar => LugarId.HasValue;
    public bool PossuiLocalLivre => LocalLivre is not null;
    public IReadOnlyList<string> EtiquetasClima => _etiquetasClima.AsReadOnly();
    public IReadOnlyList<MediaAsset> Fotos => _fotos.AsReadOnly();

    private Evento(
        string titulo,
        string slug,
        CategoriaLugar categoria,
        string descricao,
        DateTimeOffset dataInicio,
        DateTimeOffset dataFim,
        Guid? lugarId,
        LocalLivre? localLivre,
        PriceRange faixaPreco,
        string? ticketUrl,
        OrganizadorEvento? organizador,
        IEnumerable<string>? etiquetasClima,
        IEnumerable<MediaAsset>? fotos,
        bool destaque,
        bool patrocinado,
        bool simulado,
        StatusPublicacao status,
        Guid? id) : base(id ?? Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new DomainException("O título do evento é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("O slug do evento é obrigatório.");
        }

        if (!CategoriasValidasEvento.Contains(categoria))
        {
            throw new DomainException($"A categoria '{categoria}' não é válida para um Evento.");
        }

        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new DomainException("A descrição do evento é obrigatória.");
        }

        if (dataFim <= dataInicio)
        {
            throw new DomainException("A data de término do evento deve ser posterior à data de início.");
        }

        var temLugarCadastrado = lugarId.HasValue && lugarId.Value != Guid.Empty;
        var temLocalLivre = localLivre is not null;

        if (temLugarCadastrado == temLocalLivre)
        {
            throw new DomainException("O evento deve possuir exclusivamente um LugarId cadastrado OU um LocalLivre.");
        }

        ArgumentNullException.ThrowIfNull(faixaPreco);

        Titulo = titulo.Trim();
        Slug = slug.Trim().ToLowerInvariant();
        Categoria = categoria;
        Descricao = descricao.Trim();
        DataInicio = dataInicio;
        DataFim = dataFim;
        LugarId = temLugarCadastrado ? lugarId : null;
        LocalLivre = localLivre;
        FaixaPreco = faixaPreco;
        TicketUrl = string.IsNullOrWhiteSpace(ticketUrl) ? null : ticketUrl.Trim();
        Organizador = organizador;
        Destaque = destaque;
        Patrocinado = patrocinado;
        Simulado = simulado;
        Status = status;
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

    public static Evento CriarParaLugar(
        string titulo,
        string descricao,
        DateTimeOffset dataInicio,
        DateTimeOffset dataFim,
        Guid lugarId,
        PriceRange faixaPreco,
        string? ticketUrl = null,
        IEnumerable<string>? etiquetasClima = null,
        IEnumerable<MediaAsset>? fotos = null,
        string? slug = null,
        CategoriaLugar categoria = CategoriaLugar.Cultura,
        OrganizadorEvento? organizador = null,
        bool destaque = false,
        bool patrocinado = false,
        bool simulado = false,
        StatusPublicacao status = StatusPublicacao.Aprovado,
        Guid? id = null)
    {
        if (lugarId == Guid.Empty)
        {
            throw new DomainException("O identificador do Lugar vinculado deve ser válido.");
        }

        return new Evento(
            titulo,
            slug ?? GerarSlugPadrao(titulo),
            categoria,
            descricao,
            dataInicio,
            dataFim,
            lugarId,
            localLivre: null,
            faixaPreco,
            ticketUrl,
            organizador,
            etiquetasClima,
            fotos,
            destaque,
            patrocinado,
            simulado,
            status,
            id);
    }

    public static Evento CriarParaLocalLivre(
        string titulo,
        string descricao,
        DateTimeOffset dataInicio,
        DateTimeOffset dataFim,
        LocalLivre localLivre,
        PriceRange faixaPreco,
        string? ticketUrl = null,
        IEnumerable<string>? etiquetasClima = null,
        IEnumerable<MediaAsset>? fotos = null,
        string? slug = null,
        CategoriaLugar categoria = CategoriaLugar.Cultura,
        OrganizadorEvento? organizador = null,
        bool destaque = false,
        bool patrocinado = false,
        bool simulado = false,
        StatusPublicacao status = StatusPublicacao.Aprovado,
        Guid? id = null)
    {
        ArgumentNullException.ThrowIfNull(localLivre);

        return new Evento(
            titulo,
            slug ?? GerarSlugPadrao(titulo),
            categoria,
            descricao,
            dataInicio,
            dataFim,
            lugarId: null,
            localLivre,
            faixaPreco,
            ticketUrl,
            organizador,
            etiquetasClima,
            fotos,
            destaque,
            patrocinado,
            simulado,
            status,
            id);
    }

    public bool EstaAtivoEm(DateTimeOffset dataHora) =>
        Ativo && Status == StatusPublicacao.Aprovado && dataHora >= DataInicio && dataHora <= DataFim;

    public bool EstaDisponivelNaAgendaEm(DateTimeOffset agora) =>
        Ativo && Status == StatusPublicacao.Aprovado && DataFim >= agora;

    private static string GerarSlugPadrao(string titulo) =>
        string.IsNullOrWhiteSpace(titulo)
            ? string.Empty
            : string.Join('-', titulo.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));
}
