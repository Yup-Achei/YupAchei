# YupAchei — Descoberta e Curadoria Urbana

Plataforma de descoberta de estabelecimentos, eventos temporários e experiências locais em Porto Velho – RO, combinando curadoria de influenciadores parceiros, avaliações de usuários e recomendações inteligentes baseadas em intenção.

## Language

### Catálogo e Agenda

**Place**:
Estabelecimento comercial (`PlaceKind.Commercial`) ou espaço público permanente (`PlaceKind.PublicSpace`) cadastrado no catálogo (ex.: cafeteria, restaurante, bar, espaço cultural, passeio ou local esportivo).
_Avoid_: Lugar, Establishment, Venue, PointOfInterest, Store

**PlaceCategory**:
Classificação oficial aplicável a `Place` (`Cafes`, `Restaurants`, `Bars`, `Culture`, `Tours`, `Sports`) e/ou `Event` (`Culture`, `Sports`, `Music`, `Parties`, `Gastronomy`, `Theater`).
_Avoid_: CategoriaLugar, Segment, Type, Niche

**VibeTag**:
Atributo descritivo da experiência ou estrutura oferecida por um `Place` ou `Event` (ex.: `gratuito`, `familia`, `pet-friendly`, `musica-ao-vivo`, `por-do-sol`, `vista-rio`, `climatizado`).
_Avoid_: Feature, Amenity, Keyword

**UrbanRegion**:
Macrodivisão geográfica de Porto Velho usada para filtragem rápida (`Center`, `NorthZone`, `SouthZone`, `EastZone`, `WestZone`).
_Avoid_: Regioes, District, Sector, Area

**Event**:
Acontecimento temporário com data e hora de início (`StartDate`) e fim (`EndDate`), vinculado exclusivamente a um `Place` cadastrado (`PlaceId`) ou a um `FreeLocation`.
_Avoid_: Evento, Activity, Attraction, Schedule

**FreeLocation**:
Localização temporária ou espaço aberto não cadastrado como `Place` permanente no catálogo (ex.: "Mirantes I, II e III"), identificado obrigatoriamente por `Name` e `Coordinates` (`GeoCoordinates`), com `AddressText` e `Neighborhood` opcionais.
_Avoid_: LocalLivre, TemporaryPlace, UnregisteredVenue

**EventOrganizer**:
Entidade/produtora responsável pela realização de um `Event`, contendo apenas dados públicos (`Name`, `PublicContact`, `IsVerified`) em conformidade com a LGPD (sem armazenamento de CPF/CNPJ puro).
_Avoid_: OrganizadorEvento, Host, Promoter

**PublicationStatus**:
Estado editorial de um `Place` ou `Event` no catálogo (`UnderReview` como estado inicial obrigatório na criação, `Approved` para exibição pública ou `Rejected` quando reprovado na curadoria).
_Avoid_: StatusPublicacao, State, WorkflowStatus

**PlaceVerification**:
Registro de auditoria da curadoria indicando se endereço, horário e coordenadas de um `Place` foram conferidos e quais fontes públicas foram consultadas.
_Avoid_: VerificacaoLugar, Audit, Validation

### Curadoria, Avaliações e Usuários

**Partner**:
Entidade que representa um influenciador ou curador local parceiro (`Name`, `AvatarUrl`, `SocialNetwork`, `SocialProfileUrl`, `IsActive`), evitando duplicação de dados em múltiplos depoimentos.
_Avoid_: Parceiro, Influencer, Creator

**PartnerReview**:
Indicação em destaque assinada por um `Partner` (`PartnerId`) sobre um `Place` (`PlaceId`), contendo selo/nota de indicação (`BadgeOrScore`) e comentário curto (`ShortComment` de até 200 caracteres).
_Avoid_: AvaliacaoParceiro, InfluencerPost, Testimonial, SponsoredAd

**UserReview**:
Nota (`Rating` de 1 a 5 estrelas) e comentário opcional (`Comment`) registrados por um `User` autenticado sobre um `Place`, compondo a nota média (`AverageRating`) do estabelecimento.
_Avoid_: AvaliacaoUser, PartnerReview, Feedback

**User**:
Pessoa autenticada na plataforma que pode avaliar `Place`s e gerenciar seus lugares favoritos (`SavedPlaceIds`). Visitantes não autenticados consomem todo o catálogo público sem restrição.
_Avoid_: Usuario, Customer, Client, Member, Guest

### Recomendação

**RecommendationIntent**:
Expressão em texto livre enviada pelo visitante ou usuário (ex.: "Onde jantar a dois com clima intimista hoje?") para obter de 3 a 5 sugestões contextualizadas pelo agente Yupy AI (`IRecommendationAgent`).
_Avoid_: Prompt, SearchQuery
