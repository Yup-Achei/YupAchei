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

## 3. Testes Automatizados

- **`YupAchei.Domain.Tests`:** Testes unitários puros das entidades e regras de domínio (sem mocks complexos nem banco de dados).
- **`YupAchei.Application.Tests`:** Testes unitários dos casos de uso, utilizando substitutos (mocks/fakes) para interfaces de infraestrutura.
- **`YupAchei.IntegrationTests`:** Testes de integração cobrindo fluxos completos da API (`WebApplicationFactory`), persistência e integração entre camadas.

Comandos para validação:
```bash
dotnet build
dotnet test
```

Consulte [docs/architecture/backend.md](../docs/architecture/backend.md) para detalhes arquiteturais completos.

