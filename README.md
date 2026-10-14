# YupAchei 📍

**YupAchei** é um guia e recomendador de experiências locais — restaurantes, eventos, passeios e lugares — entregue como **PWA (Progressive Web App)**. A cidade de lançamento do MVP é **Porto Velho - RO**.

- Acesso 100% livre no MVP (sem login)
- Conteúdo fornecido por parceiros e influenciadores digitais
- Recomendações inteligentes via LLM/Agente (opcional no MVP)

## 🏗️ Arquitetura (Clean Architecture / DDD)

```
 ┌──────────────────────────┐
 │  YupAchei.Client (PWA)   │  React + TypeScript + Vite
 └────────────┬─────────────┘
              │ HTTP / JSON
 ┌────────────▼─────────────┐
 │     YupAchei.WebApi      │  Minimal APIs, Middlewares, DI, Swagger
 └──┬──────────┬─────────┬──┘
    │          │         │
    │   ┌──────▼──────┐  │   ┌─────────────────────────┐
    │   │ YupAchei.   │  └──►│ YupAchei.Infrastructure │ EF Core, DbContext,
    │   │ Services    │      │                         │ Migrations, Repos
    │   │ (LLM / SK)  │      └───────┬─────────┬───────┘
    │   └──────┬──────┘              │         │
    │          │                     │         │
 ┌──▼──────────▼─────────────────────▼──┐      │
 │        YupAchei.Application          │      │  Casos de uso, DTOs,
 └──────────────────┬───────────────────┘      │  Mappings, Interfaces
                    │                          │
 ┌──────────────────▼──────────────────────────▼┐
 │              YupAchei.Domain                 │  Entidades, Enums,
 │        (sem dependências externas)           │  Interfaces de Repositório
 └──────────────────────────────────────────────┘
```

| Projeto | Responsabilidade | Depende de |
|---|---|---|
| `YupAchei.Domain` | Entidades, Enums, Interfaces de Repositório | — |
| `YupAchei.Application` | Casos de uso, DTOs, Mappings, Interfaces de serviços | Domain |
| `YupAchei.Infrastructure` | EF Core, DbContext, Migrations, Repositórios, Serviços externos | Domain, Application |
| `YupAchei.Services` | Integração LLM/Agente, Semantic Kernel (opcional no MVP) | Application |
| `YupAchei.WebApi` | Minimal APIs/Controllers, Middlewares, DI, Swagger | Application, Infrastructure, Services |
| `YupAchei.Client` | Frontend React + TypeScript PWA (Vite) | WebApi (HTTP) |

## 🧰 Stack

- **Frontend:** React, TypeScript, Vite, PWA
- **Backend:** C#, .NET 8, ASP.NET Core (Minimal APIs), Swagger/OpenAPI
- **Dados:** Entity Framework Core, PostgreSQL 16
- **IA (opcional):** Semantic Kernel
- **Infra local:** Docker Compose

## 🚀 Como começar

```bash
git clone https://github.com/CunhaRo/YupAchei.git
cd YupAchei

# Criar os projetos .NET dentro das pastas existentes (referenciados em YupAchei.sln)
dotnet new classlib -n YupAchei.Domain -o src/YupAchei.Domain
dotnet new classlib -n YupAchei.Application -o src/YupAchei.Application
dotnet new classlib -n YupAchei.Infrastructure -o src/YupAchei.Infrastructure
dotnet new classlib -n YupAchei.Services -o src/YupAchei.Services
dotnet new webapi  -n YupAchei.WebApi -o src/YupAchei.WebApi

# Criar o frontend
npm create vite@latest src/YupAchei.Client -- --template react-ts

# Subir o ambiente
docker compose up -d
```

Portas: WebApi `5000`, Client `5173`, PostgreSQL `5432`.

> ⚠️ As credenciais do `docker-compose.yml` (`changeme`) são placeholders — altere-as em ambientes reais.

## 📋 Gestão do projeto

Board Jira: https://yupachei.atlassian.net/jira/software/projects/KAN/list
