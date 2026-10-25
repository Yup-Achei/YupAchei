# 0002. Padronização de Identificadores de Código em Inglês e Refinamento do Domínio

Date: 2026-10-24
Status: Accepted

## Context

Durante as primeiras iterações do repositório **YupAchei**, parte das entidades do backend (`Lugar`, `Evento`, `AvaliacaoParceiro`, `AvaliacaoUser`) estava em português, enquanto outros componentes do backend (`User`, `GeoCoordinates`, `Address`, `WeeklySchedule`, `PriceRange`, `UsefulLinks`, `MediaAsset`) e os contratos TypeScript do frontend (`Place`, `Event`, `Category`, `Tag` em `frontend/src/features/`) já estavam escritos em inglês.

Em reunião de alinhamento da equipe, foi definido que **o padrão oficial de escrita de código (identificadores, classes, propriedades, métodos, parâmetros e enums) será o inglês**, mantendo os textos de negócio, seeds de Porto Velho – RO, documentação arquitetural e mensagens de validação voltadas ao usuário em português (PT-BR).

Além disso, a revisão de modelagem de domínio identificou 5 melhorias estruturais que precisavam ser consolidadas nesse padrão:
1. Unificação do identificador amigável de URL como `Slug` tanto em `Place` quanto em `Event`.
2. Unificação da representação de faixa de preço usando o Value Object `PriceRange` (`MinAmount`, `MaxAmount`, `Currency`, `IsFree`) em `Place` e `Event`.
3. Garantia de curadoria prévia obrigatória: `Place` e `Event` nascem sempre com `Status = PublicationStatus.UnderReview` e exigem transição explícita (`Approve()`, `Reject()`, `SendToReview()`).
4. Extração da entidade `Partner` para eliminar repetição de nome e avatar em cada `PartnerReview`.
5. Adequação à LGPD em `EventOrganizer`, removendo documento fiscal/pessoal (`CNPJ`/`MEI`) e mantendo apenas dados públicos (`Name`, `PublicContact`, `IsVerified`).

## Decision

1. **Código-fonte em Inglês:** Todas as camadas do backend (`YupAchei.Domain`, `YupAchei.Application`, `YupAchei.Infrastructure`, `YupAchei.API` e testes) adotam nomenclatura estritamente em inglês, alinhada ao `GLOSSARY.md` e aos contratos do frontend:
   - Entidades: `Place`, `Event`, `Partner`, `PartnerReview`, `User`, `UserReview`.
   - Value Objects: `Address`, `GeoCoordinates`, `WeeklySchedule`, `DailyHours`, `PriceRange`, `UsefulLinks`, `MediaAsset`, `FreeLocation`, `EventOrganizer`, `PlaceVerification`.
   - Enums: `PlaceCategory`, `PlaceKind`, `PublicationStatus`, `UrbanRegion`, `SocialNetwork`.
   - Repositórios e Serviços: `IPlaceRepository`, `IEventRepository`, `IUserRepository`, `IUserReviewRepository`, `ICatalogService`, `IRecommendationService`, `IRecommendationAgent`.
2. **Preservação das Invariantes de Domínio e Regras de Porto Velho – RO:**
   - `Address` permite `Street` e `Neighborhood` opcionais (`null`) para praças e espaços públicos de Porto Velho, exigindo `City` e `State`.
   - `Place` suporta `IsOpen24Hours` e nasce em `PublicationStatus.UnderReview`.
   - `Event` exige vínculo exclusivo (`PlaceId` XOR `FreeLocation`) e nasce em `PublicationStatus.UnderReview`.

## Consequences

- **Consistência Full-Stack:** O vocabulário ubíquo em inglês (`Place`, `Event`, `Partner`, `PriceRange`, `Slug`) é idêntico entre C# (.NET 10) e TypeScript (React), eliminando traduções mentais e mapeamentos ambíguos.
- **Naturalidade na Linguagem:** Eliminam-se termos híbridos como `AvaliacaoUser` em favor de `UserReview` e `PartnerReview`.
- **Segurança e Conformidade (LGPD):** Nenhum dado cadastral sensível ou documento fiscal desnecessário é armazenado em `EventOrganizer`.
- **Governança Editorial:** Nenhum `Place` ou `Event` é publicado automaticamente sem passar por `Approve()`.
