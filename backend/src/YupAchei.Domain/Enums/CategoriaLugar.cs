namespace YupAchei.Domain.Enums;

/// <summary>
/// Categorias oficiais do catálogo para locais e eventos (alinhadas com categorias.json).
/// </summary>
public enum CategoriaLugar
{
    Cafeterias = 1,
    Restaurantes = 2,
    Bares = 3,
    Cultura = 4,
    Passeios = 5,
    Esportes = 6,
    Musica = 7,
    Festas = 8,
    Gastronomia = 9,
    Teatro = 10
}

public enum TipoLugar
{
    Publico = 1,
    Comercial = 2
}

public enum StatusPublicacao
{
    Aprovado = 1,
    EmRevisao = 2
}

public enum TipoDocumentoOrganizador
{
    Mei = 1,
    Cnpj = 2
}
