# YupAchei

Estrutura inicial de desenvolvimento do projeto **YupAchei**, composta por backend independente em **.NET 10** (Clean Architecture) e frontend em **React + TypeScript** (Vite).

---

## 📁 Estrutura do Repositório

```text
YupAchei/
├── AGENTS.md                  # Instruções operacionais para agentes e devs
├── CONTEXT.md                 # Contexto técnico e premissas confirmadas
├── README.md                  # Apresentação do projeto e guia de uso
├── .gitignore                 # Regras de exclusão do Git
├── .agents/
│   ├── rules/                 # Regras arquiteturais e limites operacionais
│   └── skills/                # Skills de desenvolvimento de agentes
├── docs/                      # Documentação técnica e arquitetural
│   ├── architecture/          # Visão geral, backend, frontend e ADRs
│   ├── development/           # Fluxo de trabalho e estratégia de testes
│   ├── agents/                # Configuração e catálogo de skills
│   └── specs/                 # Especificações funcionais futuras
├── backend/                   # Backend .NET 10 (Clean Architecture)
│   ├── AGENTS.md
│   ├── Dockerfile
│   ├── YupAchei.sln
│   ├── src/                   # Camadas Domain, Application, Infrastructure, API
│   └── tests/                 # Projetos xUnit unitários e de integração
├── frontend/                  # Frontend React + TypeScript (Vite)
│   ├── AGENTS.md
│   ├── Dockerfile
│   ├── package.json
│   ├── tsconfig.json
│   ├── vite.config.ts
│   └── src/                   # Módulos app, features, components, services, etc.
└── docker-compose.yml         # Orquestração local dos serviços
```

---

## 🛠️ Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Node.js 22+](https://nodejs.org/) e npm 10+
- [Git](https://git-scm.com/)
- [Docker](https://www.docker.com/) e Docker Compose *(opcional para execução local em containers)*

---

## 🚀 Comandos de Desenvolvimento

### Backend (.NET 10)

Executar a partir do diretório `backend/`:

```bash
# Restaurar dependências
dotnet restore

# Compilar a solução completa
dotnet build

# Executar a suíte de testes (unitários e de integração)
dotnet test

# Executar a API em modo de desenvolvimento
dotnet run --project src/YupAchei.API
```

### Frontend (React + TypeScript / Vite)

Executar a partir do diretório `frontend/`:

```bash
# Instalar dependências
npm install

# Iniciar servidor de desenvolvimento local
npm run dev

# Checagem de tipos e compilação de produção
npm run build

# Executar análise estática (linter)
npm run lint
```

### Docker Compose

> ⚠️ **Status:** *Estrutura descritiva inicial configurada. A validação completa da execução dos containers em ambiente integrado está pendente de definição de infraestrutura e serviços adicionais.*

```bash
# Construção e inicialização dos serviços (quando os requisitos forem consolidados)
docker compose build
docker compose up -d
```

---

## 🧪 Estratégia de Testes

Consulte o documento [docs/development/testing.md](docs/development/testing.md) para detalhes sobre a organização e execução de testes.

---

## 📖 Documentação Adicional

- [Contexto do Projeto](CONTEXT.md)
- [Regras para Agentes](AGENTS.md)
- [Visão Geral da Arquitetura](docs/architecture/overview.md)
- [Arquitetura do Backend](docs/architecture/backend.md)
- [Arquitetura do Frontend](docs/architecture/frontend.md)
- [Fluxo de Desenvolvimento](docs/development/workflow.md)

