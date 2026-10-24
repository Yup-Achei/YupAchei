# YupAchei — Descoberta e Curadoria Urbana

Plataforma de descoberta de estabelecimentos, eventos temporários e experiências locais, combinando curadoria de influenciadores parceiros, avaliações de usuários e recomendações inteligentes baseadas em intenção.

## Language

### Catálogo e Agenda

**Place (Lugar)**:
Estabelecimento comercial ou espaço público permanente cadastrado no catálogo (ex.: cafeteria, restaurante, bar, espaço cultural, passeio ou local esportivo).
_Avoid_: Establishment, Venue, PointOfInterest, Store

**PlaceCategory (Categoria)**:
Classificação oficial aplicável a Lugares (`Cafeterias`, `Restaurantes`, `Bares`, `Cultura`, `Passeios`, `Esportes`) e/ou Eventos (`Cultura`, `Esportes`, `Musica`, `Festas`, `Gastronomia`, `Teatro`).
_Avoid_: Segment, Type, Niche

**VibeTag (Etiqueta de Clima/Ambiente)**:
Atributo descritivo da experiência ou estrutura oferecida por um Lugar ou Evento (ex.: `gratuito`, `familia`, `pet-friendly`, `musica-ao-vivo`, `por-do-sol`, `vista-rio`, `climatizado`).
_Avoid_: Feature, Amenity, Keyword

**UrbanRegion (Região Urbana)**:
Macrodivisão geográfica da cidade usada para filtragem rápida (`Centro`, `ZonaNorte`, `ZonaSul`, `ZonaLeste`, `ZonaOeste`).
_Avoid_: District, Sector, Area

**Evento**:
Acontecimento temporário com data e hora de início e fim, vinculado exclusivamente a um Lugar cadastrado (`LugarId`) ou a um `LocalLivre`.
_Avoid_: Activity, Attraction, Schedule

**LocalLivre (Local Livre / Avulso)**:
Localização temporária ou espaço aberto não cadastrado como Lugar permanente no catálogo (ex.: "Mirantes I, II e III"), identificado obrigatoriamente por nome e coordenadas geográficas, com endereço e bairro opcionais.
_Avoid_: TemporaryPlace, UnregisteredVenue

**OrganizadorEvento (Organizador)**:
Pessoa jurídica (`MEI` ou `CNPJ`) responsável pela realização de um Evento.
_Avoid_: Host, Promoter

**StatusPublicacao (Status de Publicação)**:
Estado editorial de um Lugar ou Evento no catálogo (`Aprovado` para exibição pública ou `EmRevisao` enquanto aguarda curadoria).
_Avoid_: State,WorkflowStatus

**VerificacaoLugar (Verificação Editorial)**:
Registro de auditoria da curadoria indicando se endereço, horário e coordenadas de um Lugar foram conferidos e quais fontes públicas foram consultadas.
_Avoid_: Audit, Validation

### Curadoria, Avaliações e Usuários

**PartnerReview (Depoimento de Parceiro / Curadoria)**:
Indicação em destaque assinada por um influenciador ou parceiro local sobre um Lugar, contendo nome, avatar, rede social de origem, selo/nota de indicação e comentário curto.
_Avoid_: InfluencerPost, Testimonial, SponsoredAd

**UserReview (Avaliação de Usuário)**:
Nota (1 a 5 estrelas) e comentário opcional registrados por um Usuário autenticado sobre um Lugar, compondo a nota média do estabelecimento.
_Avoid_: PartnerReview, Feedback, Comment

**User (Usuário)**:
Pessoa autenticada na plataforma que pode avaliar Lugares e salvar itens favoritos na sua conta. Visitantes não autenticados consomem todo o catálogo público sem restrição.
_Avoid_: Customer, Client, Member, Guest

**SavedPlace (Lugar Salvo)**:
Vínculo de favoritismo entre um Usuário e um Lugar para consulta rápida posterior.
_Avoid_: Bookmark, WishlistItem, LikedPlace

### Recomendação

**RecommendationIntent (Intenção de Busca)**:
Expressão em texto livre enviada pelo visitante ou usuário (ex.: "Onde jantar a dois com clima intimista hoje?") para obter de 3 a 5 sugestões contextualizadas pelo agente Yupy AI.
_Avoid_: Prompt, SearchQuery
