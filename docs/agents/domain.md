# Domain Docs e Arquitetura

Regras de consulta e consumo da documentação técnica e de domínio pelas engineering skills.

---

## 1. Documentos Obrigatórios para Consulta Prévia

Antes de planejar ou alterar código, os agentes e desenvolvedores devem inspecionar:

- **`CONTEXT.md`** na raiz do projeto: premissas técnicas confirmadas e lacunas pendentes de definição.
- **`docs/architecture/`**:
  - `docs/architecture/overview.md`: Visão geral e limites de sistemas.
  - `docs/architecture/backend.md`: Clean Architecture do backend .NET 10.
  - `docs/architecture/frontend.md`: Arquitetura React + TypeScript.
- **`docs/architecture/decisions/`**: Registros de Decisões Arquiteturais (ADRs) aprovados.
- **`docs/agents/`**: Configurações operacionais dos agentes e rastreadores.

---

## 2. Estrutura de Arquivos

```text
YupAchei/
├── CONTEXT.md                               # Contexto de domínio e premissas
├── docs/
│   ├── architecture/
│   │   ├── overview.md                      # Visão do sistema
│   │   ├── backend.md                       # Camadas .NET 10
│   │   ├── frontend.md                      # Módulos React + TS
│   │   └── decisions/                       # ADRs do sistema
│   │       └── 0001-adocao-clean-architecture.md
│   ├── development/
│   │   ├── workflow.md                      # Fluxo de trabalho
│   │   └── testing.md                       # Diretrizes de testes
│   └── agents/                              # Configuração de skills
│       ├── issue-tracker.md
│       ├── triage-labels.md
│       └── domain.md
```

---

## 3. Respeito às Decisões (ADRs)

Se qualquer proposta ou implementação técnica conflitar com uma ADR existente, o conflito deve ser explicitamente apontado antes de qualquer alteração de código.

