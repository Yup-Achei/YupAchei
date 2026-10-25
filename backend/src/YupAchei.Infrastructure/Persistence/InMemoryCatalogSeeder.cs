using YupAchei.Domain.Entities;
using YupAchei.Domain.Enums;
using YupAchei.Domain.ValueObjects;

namespace YupAchei.Infrastructure.Persistence;

public static class InMemoryCatalogSeeder
{
    public static readonly Guid EfmmId = Guid.Parse("11111111-1111-1111-1111-111111111001");
    public static readonly Guid CaixasDaguaId = Guid.Parse("11111111-1111-1111-1111-111111111002");
    public static readonly Guid AluizioFerreiraId = Guid.Parse("11111111-1111-1111-1111-111111111003");
    public static readonly Guid ParqueDaCidadeId = Guid.Parse("11111111-1111-1111-1111-111111111005");
    public static readonly Guid ParqueCircuitoId = Guid.Parse("11111111-1111-1111-1111-111111111006");
    public static readonly Guid MercadoCulturalId = Guid.Parse("11111111-1111-1111-1111-111111111010");
    public static readonly Guid CafeMadeiraId = Guid.Parse("11111111-1111-1111-1111-111111111020");
    public static readonly Guid RemansoDoTucunareId = Guid.Parse("11111111-1111-1111-1111-111111111021");
    public static readonly Guid MiranteMadeiraBarId = Guid.Parse("11111111-1111-1111-1111-111111111022");

    public static IReadOnlyList<Place> CreateInitialPlaces()
    {
        var guiaPvhPartner = new Partner(
            "Guia Porto Velho",
            "https://picsum.photos/seed/avatar1/120/120",
            SocialNetwork.Instagram,
            "https://instagram.com/guiaportovelho");

        var rondoniaGourmetPartner = new Partner(
            "Rondônia Gourmet",
            "https://picsum.photos/seed/avatar2/120/120",
            SocialNetwork.Instagram,
            "https://instagram.com/rondoniagourmet");

        var saboresNortePartner = new Partner(
            "Sabores do Norte",
            "https://picsum.photos/seed/avatar3/120/120",
            SocialNetwork.Instagram,
            "https://instagram.com/saboresdonorte");

        var noitePvhPartner = new Partner(
            "Noite PVH",
            "https://picsum.photos/seed/avatar4/120/120",
            SocialNetwork.TikTok,
            "https://tiktok.com/@noitepvh");

        var tueToSun10To22 = new WeeklySchedule(
        [
            new DailyHours(DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(22, 0)),
            new DailyHours(DayOfWeek.Wednesday, new TimeOnly(10, 0), new TimeOnly(22, 0)),
            new DailyHours(DayOfWeek.Thursday, new TimeOnly(10, 0), new TimeOnly(22, 0)),
            new DailyHours(DayOfWeek.Friday, new TimeOnly(10, 0), new TimeOnly(22, 0)),
            new DailyHours(DayOfWeek.Saturday, new TimeOnly(10, 0), new TimeOnly(22, 0)),
            new DailyHours(DayOfWeek.Sunday, new TimeOnly(10, 0), new TimeOnly(22, 0))
        ]);

        var daily05To22 = new WeeklySchedule(
            Enum.GetValues<DayOfWeek>().Select(d => new DailyHours(d, new TimeOnly(5, 0), new TimeOnly(22, 0))));

        var cafeSchedule = new WeeklySchedule(
            Enum.GetValues<DayOfWeek>().Select(d => new DailyHours(d, new TimeOnly(8, 0), new TimeOnly(21, 0))));

        var restaurantSchedule = new WeeklySchedule(
            Enum.GetValues<DayOfWeek>().Select(d => new DailyHours(d, new TimeOnly(11, 30), new TimeOnly(23, 30))));

        var nightBarSchedule = new WeeklySchedule(
        [
            new DailyHours(DayOfWeek.Thursday, new TimeOnly(17, 30), new TimeOnly(1, 30)),
            new DailyHours(DayOfWeek.Friday, new TimeOnly(17, 30), new TimeOnly(2, 0)),
            new DailyHours(DayOfWeek.Saturday, new TimeOnly(16, 0), new TimeOnly(2, 0)),
            new DailyHours(DayOfWeek.Sunday, new TimeOnly(16, 0), new TimeOnly(23, 0))
        ]);

        var efmm = new Place(
            name: "Complexo da Estrada de Ferro Madeira-Mamoré",
            slug: "complexo-estrada-de-ferro-madeira-mamore",
            description: "Principal ponto turístico da cidade, às margens do Rio Madeira. Galpões históricos, locomotivas, deck, pista de caminhada e museu gratuito com agendamento.",
            category: PlaceCategory.Tours,
            address: new Address("Av. Farquar", "5", "Panair", UrbanRegion.Center, "Porto Velho", "RO", "76801-429"),
            coordinates: new GeoCoordinates(-8.7570, -63.9110),
            openingHours: tueToSun10To22,
            links: new UsefulLinks(email: "museuefmm@portovelho.ro.gov.br"),
            vibeTags: ["gratuito", "familia", "ar-livre", "por-do-sol", "vista-rio", "historico", "estacionamento"],
            photos: [new MediaAsset("https://picsum.photos/seed/efmm/800/600", "Complexo EFMM", isCover: true)],
            kind: PlaceKind.Public,
            priceRange: PriceRange.Free(),
            isOpen24Hours: false,
            notes: "Museu: quarta a domingo, 10h às 18h, gratuito com agendamento.",
            verification: new PlaceVerification(true, true, false, ["https://portalamazonia.com/turismo/museu-da-estrada-de-ferro-madeira-mamore-e-opcao-de-lazer-e-aprendizado-em-porto-velho/"]),
            id: EfmmId);
        efmm.RecalculateRating(4.7, 312);
        efmm.AddPartnerReview(PartnerReview.CreateWithPartner(
            efmm.Id,
            guiaPvhPartner,
            "Imperdível 4.9",
            "O pôr do sol mais bonito de Rondônia na beira do Rio Madeira."));
        efmm.Approve();

        var caixasDagua = new Place(
            name: "Praça das Três Caixas d'Água",
            slug: "praca-das-tres-caixas-dagua",
            description: "As 'Três Marias' abasteceram a cidade até os anos 1950. Cartão-postal e ponto tradicional de eventos no centro antigo.",
            category: PlaceCategory.Tours,
            address: new Address("Rua Dom Pedro II", "s/n", "Centro", UrbanRegion.Center, "Porto Velho", "RO"),
            coordinates: new GeoCoordinates(-8.7617, -63.9053),
            openingHours: WeeklySchedule.Empty(),
            vibeTags: ["gratuito", "familia", "ar-livre", "historico"],
            photos: [new MediaAsset("https://picsum.photos/seed/caixasdagua/800/600", "Três Caixas d'Água", isCover: true)],
            kind: PlaceKind.Public,
            priceRange: PriceRange.Free(),
            isOpen24Hours: true,
            id: CaixasDaguaId);
        caixasDagua.RecalculateRating(4.4, 2595);
        caixasDagua.Approve();

        var aluizioFerreira = new Place(
            name: "Praça Aluízio Ferreira",
            slug: "praca-aluizio-ferreira",
            description: "Praça ao lado das Três Caixas d'Água, palco de eventos culturais no centro.",
            category: PlaceCategory.Tours,
            address: new Address(null, "s/n", "Centro", UrbanRegion.Center, "Porto Velho", "RO"),
            coordinates: new GeoCoordinates(-8.7622, -63.9060),
            vibeTags: ["gratuito", "familia", "ar-livre"],
            photos: [new MediaAsset("https://picsum.photos/seed/aluizioferreira/800/600", "Praça Aluízio Ferreira", isCover: true)],
            kind: PlaceKind.Public,
            priceRange: PriceRange.Free(),
            isOpen24Hours: true,
            id: AluizioFerreiraId);
        aluizioFerreira.RecalculateRating(4.2, 2200);
        aluizioFerreira.Approve();

        var parqueDaCidade = new Place(
            name: "Parque da Cidade",
            slug: "parque-da-cidade",
            description: "Parque na Av. Calama com estrutura pra caminhada e atividades físicas. Aceita passeio com animais.",
            category: PlaceCategory.Sports,
            address: new Address("Rua Adaildo Feitosa", "1", "Flodoaldo Pontes Pinto", UrbanRegion.NorthZone, "Porto Velho", "RO", "76820-452"),
            coordinates: new GeoCoordinates(-8.7490, -63.8715),
            openingHours: daily05To22,
            vibeTags: ["gratuito", "ar-livre", "pet-friendly", "familia"],
            photos: [new MediaAsset("https://picsum.photos/seed/parquedacidade/800/600", "Parque da Cidade", isCover: true)],
            kind: PlaceKind.Public,
            priceRange: PriceRange.Free(),
            id: ParqueDaCidadeId);
        parqueDaCidade.RecalculateRating(4.5, 691);
        parqueDaCidade.Approve();

        var parqueCircuito = new Place(
            name: "Parque Circuito (Parque Dr. José Adelino)",
            slug: "parque-circuito",
            description: "1 km de pista de caminhada sombreada por seringueiras, campo de futebol e vôlei de areia, playground e iluminação em LED até 22h.",
            category: PlaceCategory.Sports,
            address: new Address("Av. Lauro Sodré", "2983", "Olaria", UrbanRegion.NorthZone, "Porto Velho", "RO", "76803-460"),
            coordinates: new GeoCoordinates(-8.7520, -63.8890),
            openingHours: daily05To22,
            vibeTags: ["gratuito", "ar-livre", "familia", "estacionamento"],
            photos: [new MediaAsset("https://picsum.photos/seed/parquecircuito/800/600", "Parque Circuito", isCover: true)],
            kind: PlaceKind.Public,
            priceRange: PriceRange.Free(),
            id: ParqueCircuitoId);
        parqueCircuito.RecalculateRating(4.6, 845);
        parqueCircuito.Approve();

        var mercadoCultural = new Place(
            name: "Mercado Cultural de Porto Velho",
            slug: "mercado-cultural",
            description: "Prédio histórico revitalizado no centro com gastronomia regional, café da manhã típico e apresentações musicais.",
            category: PlaceCategory.Culture,
            address: new Address("Av. Presidente Dutra", "2816", "Centro", UrbanRegion.Center, "Porto Velho", "RO"),
            coordinates: new GeoCoordinates(-8.7615, -63.9045),
            openingHours: restaurantSchedule,
            vibeTags: ["musica-ao-vivo", "comida-regional", "historico", "familia"],
            photos: [new MediaAsset("https://picsum.photos/seed/mercadocultural/800/600", "Mercado Cultural", isCover: true)],
            kind: PlaceKind.Public,
            priceRange: new PriceRange(15m, 60m),
            id: MercadoCulturalId);
        mercadoCultural.RecalculateRating(4.5, 510);
        mercadoCultural.Approve();

        var cafeMadeira = new Place(
            name: "Café Madeira Especial",
            slug: "cafe-madeira-especial",
            description: "Cafeteria aconchegante e climatizada com grãos amazônicos, doces artesanais e espaço perfeito para trabalhar ou conversar a dois.",
            category: PlaceCategory.Cafes,
            address: new Address("Av. Carlos Gomes", "1450", "São Cristóvão", UrbanRegion.Center, "Porto Velho", "RO"),
            coordinates: new GeoCoordinates(-8.7595, -63.8980),
            openingHours: cafeSchedule,
            links: new UsefulLinks("https://maps.google.com", "@cafemadeirapvh", "5569999990001"),
            vibeTags: ["climatizado", "wifi", "trabalhar", "comida-regional"],
            photos: [new MediaAsset("https://picsum.photos/seed/cafemadeira/800/600", "Café Madeira", isCover: true)],
            kind: PlaceKind.Commercial,
            priceRange: new PriceRange(18m, 55m),
            id: CafeMadeiraId);
        cafeMadeira.RecalculateRating(4.8, 184);
        cafeMadeira.AddPartnerReview(PartnerReview.CreateWithPartner(
            cafeMadeira.Id,
            rondoniaGourmetPartner,
            "Destaque 4.9",
            "Melhor café coado e tapioquinha com tucumã para começar a manhã!"));
        cafeMadeira.Approve();

        var remansoTucunare = new Place(
            name: "Remanso do Tucunaré",
            slug: "remanso-do-tucunare",
            description: "Restaurante de culinária amazônica contemporânea com ambiente intimista, peixes nobres na brasa e carta de vinhos.",
            category: PlaceCategory.Restaurants,
            address: new Address("Av. Rio Madeira", "3100", "Nova Porto Velho", UrbanRegion.NorthZone, "Porto Velho", "RO"),
            coordinates: new GeoCoordinates(-8.7450, -63.8820),
            openingHours: restaurantSchedule,
            links: new UsefulLinks("https://maps.google.com", "@remansotucunare", "5569999990002"),
            vibeTags: ["comida-regional", "climatizado", "estacionamento", "familia"],
            photos: [new MediaAsset("https://picsum.photos/seed/remansotucunare/800/600", "Remanso do Tucunaré", isCover: true)],
            kind: PlaceKind.Commercial,
            priceRange: new PriceRange(65m, 160m),
            id: RemansoDoTucunareId);
        remansoTucunare.RecalculateRating(4.7, 290);
        remansoTucunare.AddPartnerReview(PartnerReview.CreateWithPartner(
            remansoTucunare.Id,
            saboresNortePartner,
            "Top Jantar 4.8",
            "Ambiente intimista impecável para jantar a dois em Porto Velho."));
        remansoTucunare.Approve();

        var miranteBar = new Place(
            name: "Mirante Deck Bar",
            slug: "mirante-deck-bar",
            description: "Bar com vista privilegiada para o pôr do sol no Rio Madeira, drinks autorais com ingredientes da Amazônia e música ao vivo.",
            category: PlaceCategory.Bars,
            address: new Address("Rua Beira Rio", "210", "Caiari", UrbanRegion.Center, "Porto Velho", "RO"),
            coordinates: new GeoCoordinates(-8.7580, -63.9090),
            openingHours: nightBarSchedule,
            links: new UsefulLinks("https://maps.google.com", "@mirantedeckbar", "5569999990003"),
            vibeTags: ["musica-ao-vivo", "por-do-sol", "vista-rio", "ar-livre"],
            photos: [new MediaAsset("https://picsum.photos/seed/mirantebar/800/600", "Mirante Deck Bar", isCover: true)],
            kind: PlaceKind.Commercial,
            priceRange: new PriceRange(35m, 120m),
            id: MiranteMadeiraBarId);
        miranteBar.RecalculateRating(4.6, 215);
        miranteBar.AddPartnerReview(PartnerReview.CreateWithPartner(
            miranteBar.Id,
            noitePvhPartner,
            "Melhor Drink 4.8",
            "Melhor drink autoral de caju e jambu vendo o pôr do sol!"));
        miranteBar.Approve();

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

    public static IReadOnlyList<Event> CreateInitialEvents()
    {
        var offset = TimeSpan.FromHours(-4);

        var events = new List<Event>
        {
            Event.CreateForPlace(
                title: "Cinema ao Ar Livre",
                description: "Sessão gratuita de cinema na praça. Leve cadeira ou canga.",
                startDate: new DateTimeOffset(2026, 10, 24, 19, 0, 0, offset),
                endDate: new DateTimeOffset(2026, 10, 24, 21, 30, 0, offset),
                placeId: CaixasDaguaId,
                priceRange: PriceRange.Free(),
                vibeTags: ["gratuito", "familia", "ar-livre"],
                photos: [new MediaAsset("https://picsum.photos/seed/evtcinema/800/600", "Cinema ao Ar Livre", isCover: true)],
                slug: "cinema-ao-ar-livre-caixas-dagua",
                category: PlaceCategory.Culture,
                organizer: new EventOrganizer("CineClube Beradero", "@cineclubeberadero", isVerified: true),
                isFeatured: true,
                isSimulated: true),

            Event.CreateForPlace(
                title: "Feira Gastronômica Beira-Rio",
                description: "Food trucks, comida regional e música ao vivo no Complexo da EFMM.",
                startDate: new DateTimeOffset(2026, 10, 31, 16, 0, 0, offset),
                endDate: new DateTimeOffset(2026, 10, 31, 23, 0, 0, offset),
                placeId: EfmmId,
                priceRange: PriceRange.Free(),
                vibeTags: ["gratuito", "familia", "por-do-sol", "musica-ao-vivo", "comida-regional"],
                photos: [new MediaAsset("https://picsum.photos/seed/evtgastro/800/600", "Feira Gastronômica", isCover: true)],
                slug: "feira-gastronomica-efmm",
                category: PlaceCategory.Gastronomy,
                organizer: new EventOrganizer("Associação Gastronômica PVH", "@gastronomiapvh", isVerified: true),
                isFeatured: true,
                isSponsored: true,
                isSimulated: true),

            Event.CreateForPlace(
                title: "Corrida Noturna 5K",
                description: "Corrida de 5 km com kit e medalha no Parque Circuito.",
                startDate: new DateTimeOffset(2026, 11, 14, 19, 0, 0, offset),
                endDate: new DateTimeOffset(2026, 11, 14, 21, 30, 0, offset),
                placeId: ParqueCircuitoId,
                priceRange: PriceRange.StartingFrom(60m),
                ticketUrl: "https://exemplo.com/inscricao/corrida-5k",
                vibeTags: ["ar-livre"],
                photos: [new MediaAsset("https://picsum.photos/seed/evtcorrida/800/600", "Corrida Noturna", isCover: true)],
                slug: "corrida-noturna-parque-circuito",
                category: PlaceCategory.Sports,
                organizer: new EventOrganizer("Run Rondônia", "@runrondonia", isVerified: true),
                isSimulated: true),

            Event.CreateForPlace(
                title: "Noite de Música Regional",
                description: "Bandas locais no Mercado Cultural.",
                startDate: new DateTimeOffset(2026, 11, 20, 19, 0, 0, offset),
                endDate: new DateTimeOffset(2026, 11, 20, 23, 30, 0, offset),
                placeId: MercadoCulturalId,
                priceRange: PriceRange.Free(),
                vibeTags: ["gratuito", "musica-ao-vivo"],
                photos: [new MediaAsset("https://picsum.photos/seed/evtmusica/800/600", "Música Regional", isCover: true)],
                slug: "show-regional-mercado-cultural",
                category: PlaceCategory.Music,
                organizer: new EventOrganizer("Coletivo Madeira", "@coletivomadeira"),
                isSimulated: true),

            Event.CreateForFreeLocation(
                title: "Forró nos Mirantes",
                description: "Forró tradicional ao pôr do sol nos Mirantes com vista para o Rio Madeira.",
                startDate: new DateTimeOffset(2026, 11, 21, 20, 0, 0, offset),
                endDate: new DateTimeOffset(2026, 11, 22, 1, 0, 0, offset),
                freeLocation: new FreeLocation("Mirantes I, II e III", null, null, new GeoCoordinates(-8.7590, -63.9110)),
                priceRange: PriceRange.StartingFrom(20m),
                vibeTags: ["musica-ao-vivo", "vista-rio"],
                photos: [new MediaAsset("https://picsum.photos/seed/evtforro/800/600", "Forró nos Mirantes", isCover: true)],
                slug: "forro-nos-mirantes",
                category: PlaceCategory.Parties,
                organizer: new EventOrganizer("Forró do Rio", "@forrodorio"),
                isSimulated: true)
        };

        foreach (var @event in events)
        {
            @event.Approve();
        }

        return events;
    }
}
