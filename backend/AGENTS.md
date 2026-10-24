# Diretrizes para o Backend (.NET 10 / Clean Architecture)

Este documento complementa o [AGENTS.md](../AGENTS.md) da raiz com diretrizes específicas para o backend do **YupAchei**.

---

## 1. Organização em Camadas e Regras de Dependência

A solução adota rigorosamente a **Clean Architecture**:

```
        ┌─────────────────────────┐
        │       YupAchei.API      │  (Entrada HTTP, DI, Program.cs)
        └───────┬─────────┬───────┘
                │         │
    ┌───────────▼┐       ┌▼─────────────────────────┐
    │ Application│◄──────┤   YupAchei.Infrastructure │ (EF Core, Repositórios,
    └─────┬──────┘       └─────────────┬────────────┘  Integrações Externas)
          │                            │
          └──────────► ┌───────────────▼┐
                       │ YupAchei.Domain│ (Entidades, Interfaces, Enums)
                       └────────────────┘
```

1. **`YupAchei.Domain`:**
   - **Regra:** Não referencia nenhum outro projeto da solução nem pacotes de infraestrutura.
   - **Conteúdo:** Entidades de domínio, objetos de valor (Value Objects), enums, exceções de domínio e interfaces de repositório.
2. **`YupAchei.Application`:**
   - **Regra:** Depende exclusivamente de `YupAchei.Domain`.
   - **Conteúdo:** Casos de uso, comandos, consultas (CQRS se aplicável), DTOs, validadores e interfaces de serviços externos.
3. **`YupAchei.Infrastructure`:**
   - **Regra:** Depende de `YupAchei.Domain` e `YupAchei.Application`.
   - **Conteúdo:** Implementações de acesso a dados (DbContext, repositórios), clientes HTTP externos, serviços de infraestrutura.
4. **`YupAchei.API`:**
   - **Regra:** Depende de `YupAchei.Application` e `YupAchei.Infrastructure`.
   - **Conteúdo:** Ponto de entrada HTTP (Minimal APIs ou Controllers), middlewares, configuração de injeção de dependência, filtros e documentação OpenAPI/Swagger.

> **Importante:** Jamais exponha instâncias do DbContext ou entidades do Domain diretamente aos clientes HTTP. Utilize DTOs na camada de Application/API.

---

## 2. Convenções de Código .NET

- **Versão:** .NET 10 / C# 14.
- **Null Safety:** Nullable reference types habilitados (`<Nullable>enable</Nullable>`). Evite supressões com `!` sem verificação defensiva.
- **Assincronismo:** Todos os métodos que realizem I/O devem ser assíncronos (`async Task<T>`), aceitar `CancellationToken` e propagá-lo até a fonte.
- **Injeção de Dependência:** Registre dependências explicitamente na camada API usando extensões dedicadas (ex.: `AddInfrastructureServices`, `AddApplicationServices`).
- **Nomenclatura:** PascalCase para tipos e métodos; camelCase para parâmetros e campos locais; prefixo `I` para interfaces (`IRepository`).

---

## 3. Testes Automatizados e Estratégia de Testes

A suíte de testes no backend segue rigorosamente a Clean Architecture, sem introdução de dependências de teste desnecessárias:

- **`YupAchei.Domain.Tests`:** Testes unitários puros das entidades, Value Objects e invariantes de regras de negócio (execução ultrarrápida, puramente em memória, sem I/O nem dependências externas).
- **`YupAchei.Application.Tests`:** Testes unitários de serviços/casos de uso (ex.: `CatalogoService`, `RecomendacaoService`).
  - **Diretriz de Dublês:** Adotar **Fakes leves em memória** (implementações simples das interfaces de repositório como `ILugarRepository`, `IEventoRepository`), sem adicionar bibliotecas de mocking (como Moq ou NSubstitute) por padrão.
  - Reutilizar fakes existentes (como os de `Infrastructure`) somente quando não criar acoplamento indevido ou circular entre projetos; caso contrário, crie fakes locais focados no cenário do teste.
- **`YupAchei.IntegrationTests`:** Testes de integração de ponta a ponta da API via `WebApplicationFactory<Program>`.
  - **Infraestrutura:** Utiliza a infraestrutura em memória existente e o `InMemoryCatalogoSeeder`, dispensando containers Docker ou bancos externos.
  - **Foco:** Validar contratos HTTP reais, roteamento Minimal API, serialização/deserialização JSON, políticas de cache de saída (`OutputCache`) e códigos de status (`200 OK`, `400 BadRequest` para `DomainException`, `404 NotFound`).

Comandos para validação:
```bash
dotnet build backend/YupAchei.sln
dotnet test backend/YupAchei.sln
```

Consulte [docs/development/testing.md](../docs/development/testing.md) e [docs/architecture/backend.md](../docs/architecture/backend.md) para especificações completas.

