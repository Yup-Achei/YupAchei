namespace YupAchei.Application.DTOs;

public sealed record CategoriaDto(
    string Id,
    string Nome,
    IReadOnlyList<string> Tipos,
    string Icone,
    string Cor,
    int Ordem);

public sealed record TagDto(
    string Id,
    string Nome,
    string Icone);

public sealed record EnderecoDto(
    string? Logradouro,
    string Numero,
    string? Bairro,
    string Cidade,
    string Uf,
    string? Cep);

public sealed record CoordenadasDto(
    double Lat,
    double Lng);

public sealed record HorarioFuncionamentoDto(
    string Dia,
    string Abre,
    string Fecha);

public sealed record ContatoDto(
    string? Instagram,
    string? Whatsapp,
    string? Telefone,
    string? Email,
    string? Site);

public sealed record ImagensLugarDto(
    string Principal,
    IReadOnlyList<string> Galeria);

public sealed record AvaliacaoResumoDto(
    double Media,
    int Total);

public sealed record VerificacaoDto(
    bool Endereco,
    bool Horario,
    bool Coordenadas,
    IReadOnlyList<string> Fontes,
    string? Observacao = null);

public sealed record AvaliacaoParceiroDto(
    Guid Id,
    Guid ParceiroId,
    string? NomeParceiro,
    string? UrlAvatar,
    string? RedeSocial,
    string? UrlPerfilSocial,
    string SeloOuNota,
    string ComentarioCurto,
    bool EmDestaque);

public sealed record FaixaPrecoDto(
    bool Gratuito,
    decimal ValorMinimo,
    decimal ValorMaximo,
    string Moeda);

public sealed record LugarDto(
    string Id,
    string Slogan,
    string Nome,
    string CategoriaId,
    string Tipo,
    string Descricao,
    EnderecoDto Endereco,
    CoordenadasDto Coordenadas,
    FaixaPrecoDto FaixaPreco,
    bool Aberto24h,
    bool AbertoAgora,
    IReadOnlyList<HorarioFuncionamentoDto> Horarios,
    string? Observacoes,
    IReadOnlyList<string> Tags,
    ContatoDto Contato,
    ImagensLugarDto Imagens,
    AvaliacaoResumoDto Avaliacao,
    IReadOnlyList<AvaliacaoParceiroDto> AvaliacoesParceiros,
    string IndicadoPor,
    bool Patrocinado,
    string Status,
    VerificacaoDto Verificacao,
    string AtualizadoEm);

public sealed record LocalLivreDto(
    string Nome,
    string? Endereco,
    string? Bairro,
    CoordenadasDto Coordenadas);

public sealed record OrganizadorDto(
    string Nome,
    string? ContatoPublico,
    bool Verificado);

public sealed record EventoDto(
    string Id,
    string Slogan,
    bool Simulado,
    string Titulo,
    string CategoriaId,
    string Descricao,
    string Inicio,
    string Fim,
    string? LocalId,
    LocalLivreDto? LocalLivre,
    FaixaPrecoDto FaixaPreco,
    string? LinkIngresso,
    string Imagem,
    OrganizadorDto? Organizador,
    IReadOnlyList<string> Tags,
    bool Destaque,
    bool Patrocinado,
    string Status);

public sealed record RecomendacaoRequestDto(string Intencao);

public sealed record RecomendacaoResponseDto(
    string Intencao,
    IReadOnlyList<LugarDto> Sugestoes,
    bool UsouFallback,
    string Mensagem);
