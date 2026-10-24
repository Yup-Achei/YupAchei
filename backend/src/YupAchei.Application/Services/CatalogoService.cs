using YupAchei.Application.DTOs;
using YupAchei.Application.Interfaces;
using YupAchei.Domain.Entities;
using YupAchei.Domain.Enums;
using YupAchei.Domain.Repositories;

namespace YupAchei.Application.Services;

public sealed class CatalogoService : ICatalogoService
{
    private static readonly IReadOnlyList<CategoriaDto> CategoriasPadrao =
    [
        new("cafeterias", "Cafeterias", ["local"], "coffee", "#F59E0B", 1),
        new("restaurantes", "Restaurantes", ["local"], "utensils", "#F97316", 2),
        new("bares", "Bares", ["local"], "beer", "#8B5CF6", 3),
        new("cultura", "Cultura", ["local", "evento"], "palette", "#3B82F6", 4),
        new("passeios", "Passeios", ["local"], "compass", "#10B981", 5),
        new("esportes", "Esportes", ["local", "evento"], "dumbbell", "#EF4444", 6),
        new("musica", "Música", ["evento"], "music", "#EC4899", 7),
        new("festas", "Festas", ["evento"], "party-popper", "#A855F7", 8),
        new("gastronomia", "Gastronomia", ["evento"], "chef-hat", "#F97316", 9),
        new("teatro", "Teatro", ["evento"], "drama", "#6366F1", 10)
    ];

    private static readonly IReadOnlyList<TagDto> TagsPadrao =
    [
        new("gratuito", "Gratuito", "tag"),
        new("familia", "Bom pra família", "baby"),
        new("pet-friendly", "Pet friendly", "dog"),
        new("sem-pets", "Não aceita pets", "ban"),
        new("acessivel", "Acessível", "accessibility"),
        new("estacionamento", "Estacionamento", "car"),
        new("wifi", "Wi-Fi", "wifi"),
        new("musica-ao-vivo", "Música ao vivo", "mic"),
        new("ar-livre", "Ao ar livre", "trees"),
        new("por-do-sol", "Pôr do sol", "sunset"),
        new("vista-rio", "Vista pro rio", "waves"),
        new("comida-regional", "Comida regional", "fish"),
        new("vegano", "Opções veganas", "leaf"),
        new("trabalhar", "Bom pra trabalhar", "laptop"),
        new("historico", "Histórico", "landmark"),
        new("climatizado", "Climatizado", "snowflake")
    ];

    private readonly ILugarRepository _lugarRepository;
    private readonly IEventoRepository _eventoRepository;

    public CatalogoService(ILugarRepository lugarRepository, IEventoRepository eventoRepository)
    {
        _lugarRepository = lugarRepository;
        _eventoRepository = eventoRepository;
    }

    public IReadOnlyList<CategoriaDto> ListarCategorias(string? tipo = null)
    {
        if (string.IsNullOrWhiteSpace(tipo))
        {
            return CategoriasPadrao;
        }

        var tipoNormalizado = tipo.Trim().ToLowerInvariant();
        return CategoriasPadrao
            .Where(c => c.Tipos.Contains(tipoNormalizado, StringComparer.OrdinalIgnoreCase))
            .OrderBy(c => c.Ordem)
            .ToList();
    }

    public IReadOnlyList<TagDto> ListarTags() => TagsPadrao;

    public async Task<IReadOnlyList<LugarDto>> ListarLugaresAsync(
        PlaceSearchCriteria criterios,
        CancellationToken cancellationToken = default)
    {
        var referencia = criterios.DataHoraReferencia ?? DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4));
        var lugares = await _lugarRepository.BuscarAsync(criterios, cancellationToken);

        return lugares
            .Where(l => l.EstaAprovado)
            .Select(l => MapearLugarParaDto(l, referencia))
            .ToList();
    }

    public async Task<LugarDto?> ObterLugarPorIdOuSlugAsync(
        string idOuSlug,
        DateTimeOffset? referencia = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idOuSlug))
        {
            return null;
        }

        Lugar? lugar = null;
        if (Guid.TryParse(idOuSlug, out var id))
        {
            lugar = await _lugarRepository.ObterPorIdAsync(id, cancellationToken);
        }

        lugar ??= await _lugarRepository.ObterPorSlugAsync(idOuSlug.Trim().ToLowerInvariant(), cancellationToken);

        if (lugar is null || !lugar.EstaAprovado)
        {
            return null;
        }

        var dataReferencia = referencia ?? DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-4));
        return MapearLugarParaDto(lugar, dataReferencia);
    }

    public async Task<IReadOnlyList<EventoDto>> ListarEventosAtivosAsync(
        EventSearchCriteria criterios,
        CancellationToken cancellationToken = default)
    {
        var eventos = await _eventoRepository.BuscarAtivosAsync(criterios, cancellationToken);
        return eventos
            .OrderBy(e => e.DataInicio)
            .Select(MapearEventoParaDto)
            .ToList();
    }

    public async Task<EventoDto?> ObterEventoPorIdOuSlugAsync(
        string idOuSlug,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idOuSlug))
        {
            return null;
        }

        if (Guid.TryParse(idOuSlug, out var id))
        {
            var porId = await _eventoRepository.ObterPorIdAsync(id, cancellationToken);
            if (porId is not null)
            {
                return MapearEventoParaDto(porId);
            }
        }

        var ativos = await _eventoRepository.BuscarAtivosAsync(new EventSearchCriteria(), cancellationToken);
        var porSlug = ativos.FirstOrDefault(e =>
            string.Equals(e.Slug, idOuSlug.Trim(), StringComparison.OrdinalIgnoreCase) ||
            string.Equals(e.Id.ToString(), idOuSlug.Trim(), StringComparison.OrdinalIgnoreCase));

        return porSlug is null ? null : MapearEventoParaDto(porSlug);
    }

    internal static LugarDto MapearLugarParaDto(Lugar lugar, DateTimeOffset referencia)
    {
        var fotoPrincipal = lugar.Fotos.FirstOrDefault(f => f.EhCapa)?.Url
                            ?? lugar.Fotos.FirstOrDefault()?.Url
                            ?? "https://picsum.photos/seed/yupachei/800/600";

        var galeria = lugar.Fotos.Where(f => !f.EhCapa).Select(f => f.Url).ToList();

        return new LugarDto(
            Id: lugar.Id.ToString(),
            Slug: lugar.Slogan,
            Nome: lugar.Nome,
            CategoriaId: MapearCategoriaParaSlug(lugar.Categoria),
            Tipo: lugar.Tipo == TipoLugar.Publico ? "publico" : "comercial",
            Descricao: lugar.Descricao,
            Endereco: new EnderecoDto(
                lugar.Endereco.Logradouro,
                lugar.Endereco.Numero,
                lugar.Endereco.Bairro,
                lugar.Endereco.Cidade,
                lugar.Endereco.Estado,
                lugar.Endereco.Cep),
            Coordenadas: new CoordenadasDto(lugar.Coordenadas.Latitude, lugar.Coordenadas.Longitude),
            FaixaPreco: lugar.FaixaPreco,
            Aberto24h: lugar.Aberto24h,
            AbertoAgora: lugar.EstaAbertoEm(referencia),
            Horarios: lugar.HorarioFuncionamento.Horarios
                .Select(h => new HorarioFuncionamentoDto(
                    MapearDiaSemana(h.DiaSemana),
                    h.HorarioAbertura.ToString("HH:mm"),
                    h.HorarioFechamento.ToString("HH:mm")))
                .ToList(),
            Observacoes: lugar.Observacoes,
            Tags: lugar.EtiquetasClima,
            Contato: new ContatoDto(
                lugar.Links.PerfilInstagram,
                lugar.Links.NumeroWhatsApp,
                lugar.Links.Telefone,
                lugar.Links.Email,
                lugar.Links.UrlSite),
            Imagens: new ImagensLugarDto(fotoPrincipal, galeria),
            Avaliacao: new AvaliacaoResumoDto(lugar.NotaMedia, lugar.QuantidadeAvaliacoes),
            AvaliacoesParceiros: lugar.AvaliacoesParceiros
                .Select(p => new AvaliacaoParceiroDto(
                    p.Id,
                    p.NomeParceiro,
                    p.UrlAvatar,
                    p.RedeSocial.ToString().ToLowerInvariant(),
                    p.UrlPerfilSocial,
                    p.SeloOuNota,
                    p.ComentarioCurto,
                    p.EmDestaque))
                .ToList(),
            IndicadoPor: lugar.IndicadoPor,
            Patrocinado: lugar.Patrocinado,
            Status: lugar.Status == StatusPublicacao.Aprovado ? "aprovado" : "em_revisao",
            Verificacao: new VerificacaoDto(
                lugar.Verificacao.EnderecoVerificado,
                lugar.Verificacao.HorarioVerificado,
                lugar.Verificacao.CoordenadasVerificadas,
                lugar.Verificacao.Fontes,
                lugar.Verificacao.Observacao),
            AtualizadoEm: lugar.AtualizadoEm.ToString("yyyy-MM-ddTHH:mm:sszzz"));
    }

    internal static EventoDto MapearEventoParaDto(Evento evento)
    {
        var imagem = evento.Fotos.FirstOrDefault()?.Url ?? "https://picsum.photos/seed/evento/800/600";

        return new EventoDto(
            Id: evento.Id.ToString(),
            Slug: evento.Slug,
            Simulado: evento.Simulado,
            Titulo: evento.Titulo,
            CategoriaId: MapearCategoriaParaSlug(evento.Categoria),
            Descricao: evento.Descricao,
            Inicio: evento.DataInicio.ToString("yyyy-MM-ddTHH:mm:sszzz"),
            Fim: evento.DataFim.ToString("yyyy-MM-ddTHH:mm:sszzz"),
            LocalId: evento.LugarId?.ToString(),
            LocalLivre: evento.LocalLivre is null
                ? null
                : new LocalLivreDto(
                    evento.LocalLivre.Nome,
                    evento.LocalLivre.Endereco,
                    evento.LocalLivre.Bairro,
                    new CoordenadasDto(evento.LocalLivre.Coordenadas.Latitude, evento.LocalLivre.Coordenadas.Longitude)),
            Preco: new PrecoEventoDto(
                evento.FaixaPreco.Gratuito,
                evento.FaixaPreco.Gratuito ? null : evento.FaixaPreco.ValorMinimo,
                evento.FaixaPreco.Moeda),
            LinkIngresso: evento.TicketUrl,
            Imagem: imagem,
            Organizador: evento.Organizador is null
                ? null
                : new OrganizadorDto(
                    evento.Organizador.Nome,
                    evento.Organizador.TipoDocumento == TipoDocumentoOrganizador.Mei ? "MEI" : "CNPJ",
                    evento.Organizador.Documento),
            Tags: evento.EtiquetasClima,
            Destaque: evento.Destaque,
            Patrocinado: evento.Patrocinado,
            Status: evento.Status == StatusPublicacao.Aprovado ? "aprovado" : "em_revisao");
    }

    public static string MapearCategoriaParaSlug(CategoriaLugar categoria) => categoria switch
    {
        CategoriaLugar.Cafeterias => "cafeterias",
        CategoriaLugar.Restaurantes => "restaurantes",
        CategoriaLugar.Bares => "bares",
        CategoriaLugar.Cultura => "cultura",
        CategoriaLugar.Passeios => "passeios",
        CategoriaLugar.Esportes => "esportes",
        CategoriaLugar.Musica => "musica",
        CategoriaLugar.Festas => "festas",
        CategoriaLugar.Gastronomia => "gastronomia",
        CategoriaLugar.Teatro => "teatro",
        _ => "passeios"
    };

    public static CategoriaLugar? TentarConverterCategoria(string? slug) =>
        slug?.Trim().ToLowerInvariant() switch
        {
            "cafeterias" => CategoriaLugar.Cafeterias,
            "restaurantes" => CategoriaLugar.Restaurantes,
            "bares" => CategoriaLugar.Bares,
            "cultura" => CategoriaLugar.Cultura,
            "passeios" => CategoriaLugar.Passeios,
            "esportes" => CategoriaLugar.Esportes,
            "musica" or "música" => CategoriaLugar.Musica,
            "festas" => CategoriaLugar.Festas,
            "gastronomia" => CategoriaLugar.Gastronomia,
            "teatro" => CategoriaLugar.Teatro,
            _ => null
        };

    private static string MapearDiaSemana(DayOfWeek dia) => dia switch
    {
        DayOfWeek.Monday => "seg",
        DayOfWeek.Tuesday => "ter",
        DayOfWeek.Wednesday => "qua",
        DayOfWeek.Thursday => "qui",
        DayOfWeek.Friday => "sex",
        DayOfWeek.Saturday => "sab",
        DayOfWeek.Sunday => "dom",
        _ => "seg"
    };
}
