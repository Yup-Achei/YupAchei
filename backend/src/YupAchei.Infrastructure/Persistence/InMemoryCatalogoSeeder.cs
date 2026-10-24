using YupAchei.Domain.Entities;
using YupAchei.Domain.Enums;
using YupAchei.Domain.ValueObjects;

namespace YupAchei.Infrastructure.Persistence;

public static class InMemoryCatalogoSeeder
{
    public static readonly Guid IdEfmm = Guid.Parse("11111111-1111-1111-1111-111111111001");
    public static readonly Guid IdCaixasDagua = Guid.Parse("11111111-1111-1111-1111-111111111002");
    public static readonly Guid IdAluizioFerreira = Guid.Parse("11111111-1111-1111-1111-111111111003");
    public static readonly Guid IdParqueDaCidade = Guid.Parse("11111111-1111-1111-1111-111111111005");
    public static readonly Guid IdParqueCircuito = Guid.Parse("11111111-1111-1111-1111-111111111006");
    public static readonly Guid IdMercadoCultural = Guid.Parse("11111111-1111-1111-1111-111111111010");
    public static readonly Guid IdCafeMadeira = Guid.Parse("11111111-1111-1111-1111-111111111020");
    public static readonly Guid IdRemansoDoTucunare = Guid.Parse("11111111-1111-1111-1111-111111111021");
    public static readonly Guid IdMiranteMadeiraBar = Guid.Parse("11111111-1111-1111-1111-111111111022");

    public static IReadOnlyList<Lugar> CriarLugaresIniciais()
    {
        var parceiroGuiaPvh = new Parceiro(
            "Guia Porto Velho",
            "https://picsum.photos/seed/avatar1/120/120",
            SocialNetwork.Instagram,
            "https://instagram.com/guiaportovelho");

        var parceiroRondoniaGourmet = new Parceiro(
            "Rondônia Gourmet",
            "https://picsum.photos/seed/avatar2/120/120",
            SocialNetwork.Instagram,
            "https://instagram.com/rondoniagourmet");

        var parceiroSaboresNorte = new Parceiro(
            "Sabores do Norte",
            "https://picsum.photos/seed/avatar3/120/120",
            SocialNetwork.Instagram,
            "https://instagram.com/saboresdonorte");

        var parceiroNoitePvh = new Parceiro(
            "Noite PVH",
            "https://picsum.photos/seed/avatar4/120/120",
            SocialNetwork.TikTok,
            "https://tiktok.com/@noitepvh");

        var horarioTerADom10As22 = new WeeklySchedule(
        [
            new DailyHours(DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(22, 0)),
            new DailyHours(DayOfWeek.Wednesday, new TimeOnly(10, 0), new TimeOnly(22, 0)),
            new DailyHours(DayOfWeek.Thursday, new TimeOnly(10, 0), new TimeOnly(22, 0)),
            new DailyHours(DayOfWeek.Friday, new TimeOnly(10, 0), new TimeOnly(22, 0)),
            new DailyHours(DayOfWeek.Saturday, new TimeOnly(10, 0), new TimeOnly(22, 0)),
            new DailyHours(DayOfWeek.Sunday, new TimeOnly(10, 0), new TimeOnly(22, 0))
        ]);

        var horarioDiario05As22 = new WeeklySchedule(
            Enum.GetValues<DayOfWeek>().Select(d => new DailyHours(d, new TimeOnly(5, 0), new TimeOnly(22, 0))));

        var horarioComercialCafe = new WeeklySchedule(
            Enum.GetValues<DayOfWeek>().Select(d => new DailyHours(d, new TimeOnly(8, 0), new TimeOnly(21, 0))));

        var horarioRestauranteJantar = new WeeklySchedule(
            Enum.GetValues<DayOfWeek>().Select(d => new DailyHours(d, new TimeOnly(11, 30), new TimeOnly(23, 30))));

        var horarioBarNoturno = new WeeklySchedule(
        [
            new DailyHours(DayOfWeek.Thursday, new TimeOnly(17, 30), new TimeOnly(1, 30)),
            new DailyHours(DayOfWeek.Friday, new TimeOnly(17, 30), new TimeOnly(2, 0)),
            new DailyHours(DayOfWeek.Saturday, new TimeOnly(16, 0), new TimeOnly(2, 0)),
            new DailyHours(DayOfWeek.Sunday, new TimeOnly(16, 0), new TimeOnly(23, 0))
        ]);

        var efmm = new Lugar(
            nome: "Complexo da Estrada de Ferro Madeira-Mamoré",
            slogan: "complexo-estrada-de-ferro-madeira-mamore",
            descricao: "Principal ponto turístico da cidade, às margens do Rio Madeira. Galpões históricos, locomotivas, deck, pista de caminhada e museu gratuito com agendamento.",
            categoria: CategoriaLugar.Passeios,
            endereco: new Address("Av. Farquar", "5", "Panair", UrbanRegion.Centro, "Porto Velho", "RO", "76801-429"),
            coordenadas: new GeoCoordinates(-8.7570, -63.9110),
            horarioFuncionamento: horarioTerADom10As22,
            links: new UsefulLinks(email: "museuefmm@portovelho.ro.gov.br"),
            etiquetasClima: ["gratuito", "familia", "ar-livre", "por-do-sol", "vista-rio", "historico", "estacionamento"],
            fotos: [new MediaAsset("https://picsum.photos/seed/efmm/800/600", "Complexo EFMM", ehCapa: true)],
            tipo: TipoLugar.Publico,
            faixaPreco: PriceRange.Gratis(),
            aberto24h: false,
            observacoes: "Museu: quarta a domingo, 10h às 18h, gratuito com agendamento.",
            verificacao: new VerificacaoLugar(true, true, false, ["https://portalamazonia.com/turismo/museu-da-estrada-de-ferro-madeira-mamore-e-opcao-de-lazer-e-aprendizado-em-porto-velho/"]),
            id: IdEfmm);
        efmm.RecalcularNota(4.7, 312);
        efmm.AdicionarAvaliacaoParceiro(AvaliacaoParceiro.CriarComParceiro(
            efmm.Id,
            parceiroGuiaPvh,
            "Imperdível 4.9",
            "O pôr do sol mais bonito de Rondônia na beira do Rio Madeira."));
        efmm.Aprovar();

        var caixasDagua = new Lugar(
            nome: "Praça das Três Caixas d'Água",
            slogan: "praca-das-tres-caixas-dagua",
            descricao: "As 'Três Marias' abasteceram a cidade até os anos 1950. Cartão-postal e ponto tradicional de eventos no centro antigo.",
            categoria: CategoriaLugar.Passeios,
            endereco: new Address("Rua Dom Pedro II", "s/n", "Centro", UrbanRegion.Centro, "Porto Velho", "RO"),
            coordenadas: new GeoCoordinates(-8.7617, -63.9053),
            horarioFuncionamento: WeeklySchedule.Vazia(),
            etiquetasClima: ["gratuito", "familia", "ar-livre", "historico"],
            fotos: [new MediaAsset("https://picsum.photos/seed/caixasdagua/800/600", "Três Caixas d'Água", ehCapa: true)],
            tipo: TipoLugar.Publico,
            faixaPreco: PriceRange.Gratis(),
            aberto24h: true,
            id: IdCaixasDagua);
        caixasDagua.RecalcularNota(4.4, 2595);
        caixasDagua.Aprovar();

        var aluizioFerreira = new Lugar(
            nome: "Praça Aluízio Ferreira",
            slogan: "praca-aluizio-ferreira",
            descricao: "Praça ao lado das Três Caixas d'Água, palco de eventos culturais no centro.",
            categoria: CategoriaLugar.Passeios,
            endereco: new Address(null, "s/n", "Centro", UrbanRegion.Centro, "Porto Velho", "RO"),
            coordenadas: new GeoCoordinates(-8.7622, -63.9060),
            etiquetasClima: ["gratuito", "familia", "ar-livre"],
            fotos: [new MediaAsset("https://picsum.photos/seed/aluizioferreira/800/600", "Praça Aluízio Ferreira", ehCapa: true)],
            tipo: TipoLugar.Publico,
            faixaPreco: PriceRange.Gratis(),
            aberto24h: true,
            id: IdAluizioFerreira);
        aluizioFerreira.RecalcularNota(4.2, 2200);
        aluizioFerreira.Aprovar();

        var parqueDaCidade = new Lugar(
            nome: "Parque da Cidade",
            slogan: "parque-da-cidade",
            descricao: "Parque na Av. Calama com estrutura pra caminhada e atividades físicas. Aceita passeio com animais.",
            categoria: CategoriaLugar.Esportes,
            endereco: new Address("Rua Adaildo Feitosa", "1", "Flodoaldo Pontes Pinto", UrbanRegion.ZonaNorte, "Porto Velho", "RO", "76820-452"),
            coordenadas: new GeoCoordinates(-8.7490, -63.8715),
            horarioFuncionamento: horarioDiario05As22,
            etiquetasClima: ["gratuito", "ar-livre", "pet-friendly", "familia"],
            fotos: [new MediaAsset("https://picsum.photos/seed/parquedacidade/800/600", "Parque da Cidade", ehCapa: true)],
            tipo: TipoLugar.Publico,
            faixaPreco: PriceRange.Gratis(),
            id: IdParqueDaCidade);
        parqueDaCidade.RecalcularNota(4.5, 691);
        parqueDaCidade.Aprovar();

        var parqueCircuito = new Lugar(
            nome: "Parque Circuito (Parque Dr. José Adelino)",
            slogan: "parque-circuito",
            descricao: "1 km de pista de caminhada sombreada por seringueiras, campo de futebol e vôlei de areia, playground e iluminação em LED até 22h.",
            categoria: CategoriaLugar.Esportes,
            endereco: new Address("Av. Lauro Sodré", "2983", "Olaria", UrbanRegion.ZonaNorte, "Porto Velho", "RO", "76803-460"),
            coordenadas: new GeoCoordinates(-8.7520, -63.8890),
            horarioFuncionamento: horarioDiario05As22,
            etiquetasClima: ["gratuito", "ar-livre", "familia", "estacionamento"],
            fotos: [new MediaAsset("https://picsum.photos/seed/parquecircuito/800/600", "Parque Circuito", ehCapa: true)],
            tipo: TipoLugar.Publico,
            faixaPreco: PriceRange.Gratis(),
            id: IdParqueCircuito);
        parqueCircuito.RecalcularNota(4.6, 845);
        parqueCircuito.Aprovar();

        var mercadoCultural = new Lugar(
            nome: "Mercado Cultural de Porto Velho",
            slogan: "mercado-cultural",
            descricao: "Prédio histórico revitalizado no centro com gastronomia regional, café da manhã típico e apresentações musicais.",
            categoria: CategoriaLugar.Cultura,
            endereco: new Address("Av. Presidente Dutra", "2816", "Centro", UrbanRegion.Centro, "Porto Velho", "RO"),
            coordenadas: new GeoCoordinates(-8.7615, -63.9045),
            horarioFuncionamento: horarioRestauranteJantar,
            etiquetasClima: ["musica-ao-vivo", "comida-regional", "historico", "familia"],
            fotos: [new MediaAsset("https://picsum.photos/seed/mercadocultural/800/600", "Mercado Cultural", ehCapa: true)],
            tipo: TipoLugar.Publico,
            faixaPreco: new PriceRange(15m, 60m),
            id: IdMercadoCultural);
        mercadoCultural.RecalcularNota(4.5, 510);
        mercadoCultural.Aprovar();

        var cafeMadeira = new Lugar(
            nome: "Café Madeira Especial",
            slogan: "cafe-madeira-especial",
            descricao: "Cafeteria aconchegante e climatizada com grãos amazônicos, doces artesanais e espaço perfeito para trabalhar ou conversar a dois.",
            categoria: CategoriaLugar.Cafeterias,
            endereco: new Address("Av. Carlos Gomes", "1450", "São Cristóvão", UrbanRegion.Centro, "Porto Velho", "RO"),
            coordenadas: new GeoCoordinates(-8.7595, -63.8980),
            horarioFuncionamento: horarioComercialCafe,
            links: new UsefulLinks("https://maps.google.com", "@cafemadeirapvh", "5569999990001"),
            etiquetasClima: ["climatizado", "wifi", "trabalhar", "comida-regional"],
            fotos: [new MediaAsset("https://picsum.photos/seed/cafemadeira/800/600", "Café Madeira", ehCapa: true)],
            tipo: TipoLugar.Comercial,
            faixaPreco: new PriceRange(18m, 55m),
            id: IdCafeMadeira);
        cafeMadeira.RecalcularNota(4.8, 184);
        cafeMadeira.AdicionarAvaliacaoParceiro(AvaliacaoParceiro.CriarComParceiro(
            cafeMadeira.Id,
            parceiroRondoniaGourmet,
            "Destaque 4.9",
            "Melhor café coado e tapioquinha com tucumã para começar a manhã!"));
        cafeMadeira.Aprovar();

        var remansoTucunare = new Lugar(
            nome: "Remanso do Tucunaré",
            slogan: "remanso-do-tucunare",
            descricao: "Restaurante de culinária amazônica contemporânea com ambiente intimista, peixes nobres na brasa e carta de vinhos.",
            categoria: CategoriaLugar.Restaurantes,
            endereco: new Address("Av. Rio Madeira", "3100", "Nova Porto Velho", UrbanRegion.ZonaNorte, "Porto Velho", "RO"),
            coordenadas: new GeoCoordinates(-8.7450, -63.8820),
            horarioFuncionamento: horarioRestauranteJantar,
            links: new UsefulLinks("https://maps.google.com", "@remansotucunare", "5569999990002"),
            etiquetasClima: ["comida-regional", "climatizado", "estacionamento", "familia"],
            fotos: [new MediaAsset("https://picsum.photos/seed/remansotucunare/800/600", "Remanso do Tucunaré", ehCapa: true)],
            tipo: TipoLugar.Comercial,
            faixaPreco: new PriceRange(65m, 160m),
            id: IdRemansoDoTucunare);
        remansoTucunare.RecalcularNota(4.7, 290);
        remansoTucunare.AdicionarAvaliacaoParceiro(AvaliacaoParceiro.CriarComParceiro(
            remansoTucunare.Id,
            parceiroSaboresNorte,
            "Top Jantar 4.8",
            "Ambiente intimista impecável para jantar a dois em Porto Velho."));
        remansoTucunare.Aprovar();

        var miranteBar = new Lugar(
            nome: "Mirante Deck Bar",
            slogan: "mirante-deck-bar",
            descricao: "Bar com vista privilegiada para o pôr do sol no Rio Madeira, drinks autorais com ingredientes da Amazônia e música ao vivo.",
            categoria: CategoriaLugar.Bares,
            endereco: new Address("Rua Beira Rio", "210", "Caiari", UrbanRegion.Centro, "Porto Velho", "RO"),
            coordenadas: new GeoCoordinates(-8.7580, -63.9090),
            horarioFuncionamento: horarioBarNoturno,
            links: new UsefulLinks("https://maps.google.com", "@mirantedeckbar", "5569999990003"),
            etiquetasClima: ["musica-ao-vivo", "por-do-sol", "vista-rio", "ar-livre"],
            fotos: [new MediaAsset("https://picsum.photos/seed/mirantebar/800/600", "Mirante Deck Bar", ehCapa: true)],
            tipo: TipoLugar.Comercial,
            faixaPreco: new PriceRange(35m, 120m),
            id: IdMiranteMadeiraBar);
        miranteBar.RecalcularNota(4.6, 215);
        miranteBar.AdicionarAvaliacaoParceiro(AvaliacaoParceiro.CriarComParceiro(
            miranteBar.Id,
            parceiroNoitePvh,
            "Melhor Drink 4.8",
            "Melhor drink autoral de caju e jambu vendo o pôr do sol!"));
        miranteBar.Aprovar();

        return
        [
            efmm,
            caixasDagua,
            aluizioFerreira,
            parqueDaCidade,
            parqueCircuito,
            mercadoCultural,
            cafeMadeira,
            remansoTucunare,
            miranteBar
        ];
    }

    public static IReadOnlyList<Evento> CriarEventosIniciais()
    {
        var fuso = TimeSpan.FromHours(-4);

        var eventos = new List<Evento>
        {
            Evento.CriarParaLugar(
                titulo: "Cinema ao Ar Livre",
                descricao: "Sessão gratuita de cinema na praça. Leve cadeira ou canga.",
                dataInicio: new DateTimeOffset(2026, 10, 24, 19, 0, 0, fuso),
                dataFim: new DateTimeOffset(2026, 10, 24, 21, 30, 0, fuso),
                lugarId: IdCaixasDagua,
                faixaPreco: PriceRange.Gratis(),
                etiquetasClima: ["gratuito", "familia", "ar-livre"],
                fotos: [new MediaAsset("https://picsum.photos/seed/evtcinema/800/600", "Cinema ao Ar Livre", ehCapa: true)],
                slogan: "cinema-ao-ar-livre-caixas-dagua",
                categoria: CategoriaLugar.Cultura,
                organizador: new OrganizadorEvento("CineClube Beradero", "@cineclubeberadero", verificado: true),
                destaque: true,
                simulado: true),

            Evento.CriarParaLugar(
                titulo: "Feira Gastronômica Beira-Rio",
                descricao: "Food trucks, comida regional e música ao vivo no Complexo da EFMM.",
                dataInicio: new DateTimeOffset(2026, 10, 31, 16, 0, 0, fuso),
                dataFim: new DateTimeOffset(2026, 10, 31, 23, 0, 0, fuso),
                lugarId: IdEfmm,
                faixaPreco: PriceRange.Gratis(),
                etiquetasClima: ["gratuito", "familia", "por-do-sol", "musica-ao-vivo", "comida-regional"],
                fotos: [new MediaAsset("https://picsum.photos/seed/evtgastro/800/600", "Feira Gastronômica", ehCapa: true)],
                slogan: "feira-gastronomica-efmm",
                categoria: CategoriaLugar.Gastronomia,
                organizador: new OrganizadorEvento("Associação Gastronômica PVH", "@gastronomiapvh", verificado: true),
                destaque: true,
                patrocinado: true,
                simulado: true),

            Evento.CriarParaLugar(
                titulo: "Corrida Noturna 5K",
                descricao: "Corrida de 5 km com kit e medalha no Parque Circuito.",
                dataInicio: new DateTimeOffset(2026, 11, 14, 19, 0, 0, fuso),
                dataFim: new DateTimeOffset(2026, 11, 14, 21, 30, 0, fuso),
                lugarId: IdParqueCircuito,
                faixaPreco: PriceRange.APartirDe(60m),
                ticketUrl: "https://exemplo.com/inscricao/corrida-5k",
                etiquetasClima: ["ar-livre"],
                fotos: [new MediaAsset("https://picsum.photos/seed/evtcorrida/800/600", "Corrida Noturna", ehCapa: true)],
                slogan: "corrida-noturna-parque-circuito",
                categoria: CategoriaLugar.Esportes,
                organizador: new OrganizadorEvento("Run Rondônia", "@runrondonia", verificado: true),
                simulado: true),

            Evento.CriarParaLugar(
                titulo: "Noite de Música Regional",
                descricao: "Bandas locais no Mercado Cultural.",
                dataInicio: new DateTimeOffset(2026, 11, 20, 19, 0, 0, fuso),
                dataFim: new DateTimeOffset(2026, 11, 20, 23, 30, 0, fuso),
                lugarId: IdMercadoCultural,
                faixaPreco: PriceRange.Gratis(),
                etiquetasClima: ["gratuito", "musica-ao-vivo"],
                fotos: [new MediaAsset("https://picsum.photos/seed/evtmusica/800/600", "Música Regional", ehCapa: true)],
                slogan: "show-regional-mercado-cultural",
                categoria: CategoriaLugar.Musica,
                organizador: new OrganizadorEvento("Coletivo Madeira", "@coletivomadeira"),
                simulado: true),

            Evento.CriarParaLocalLivre(
                titulo: "Forró nos Mirantes",
                descricao: "Forró tradicional ao pôr do sol nos Mirantes com vista para o Rio Madeira.",
                dataInicio: new DateTimeOffset(2026, 11, 21, 20, 0, 0, fuso),
                dataFim: new DateTimeOffset(2026, 11, 22, 1, 0, 0, fuso),
                localLivre: new LocalLivre("Mirantes I, II e III", null, null, new GeoCoordinates(-8.7590, -63.9110)),
                faixaPreco: PriceRange.APartirDe(20m),
                etiquetasClima: ["musica-ao-vivo", "vista-rio"],
                fotos: [new MediaAsset("https://picsum.photos/seed/evtforro/800/600", "Forró nos Mirantes", ehCapa: true)],
                slogan: "forro-nos-mirantes",
                categoria: CategoriaLugar.Festas,
                organizador: new OrganizadorEvento("Forró do Rio", "@forrodorio"),
                simulado: true)
        };

        foreach (var evento in eventos)
        {
            evento.Aprovar();
        }

        return eventos;
    }
}
