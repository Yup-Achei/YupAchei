# Estratégia de Testes — YupAchei

Este documento orienta a estratégia de qualidade e automação de testes do projeto **YupAchei**.

---

## 1. Pirâmide de Testes

A suíte de testes do projeto é estruturada de forma a balancear velocidade, isolamento e confiabilidade:

```
        / \
       /   \        Testes de Integração (API / Persistência)
      /─────\
     /       \      Testes Unitários de Casos de Uso (Application)
    /─────────\
   /           \    Testes Unitários de Regras de Domínio (Domain)
  /─────────────\
```

---

## 2. Testes de Backend (.NET 10 / xUnit)

### 2.1 Testes Unitários de Domínio (`YupAchei.Domain.Tests`)
- **Foco:** Entidades, Value Objects, validações de invariantes de negócio e cálculos de domínio.
- **Isolamento:** Execução ultrarrápida, puramente em memória, sem dependência de banco de dados, mocks externos ou frameworks de I/O.

### 2.2 Testes Unitários de Aplicação (`YupAchei.Application.Tests`)
- **Foco:** Orquestração de casos de uso (Handlers), validação de comandos e mapeamentos de DTOs.
- **Isolamento:** Uso de dublês de teste (mocks/fakes) para simular interfaces de repositórios e serviços de infraestrutura.

### 2.3 Testes de Integração (`YupAchei.IntegrationTests`)
- **Foco:** Roteamento de endpoints HTTP, serialização/deserialização JSON, injeção de dependência e persistência real (usando containers de teste ou banco de dados em memória isolado).
- **Ferramentas:** `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`).

**Execução:**
```bash
cd backend
dotnet test
```

---

## 3. Testes de Frontend (React + TypeScript)

### 3.1 Verificação de Tipos e Análise Estática
- Validação estrita do compilador TypeScript (`tsc -b`).
- Análise de qualidade de código e conformidade com linter (`oxlint` / `eslint`).

### 3.2 Testes Unitários e de Componentes
- Testes de renderização de componentes com React Testing Library e Vitest à medida que componentes e interações de interface forem implementados. A suíte inicial já usa Vitest, Testing Library e jsdom. Os arquivos ficam em `frontend/src/**/*.test.ts` e `frontend/src/**/*.test.tsx`. A preparação comum está em `frontend/src/test/setup.ts`, carregada por `frontend/vitest.config.ts`.

**Execução:**
```bash
cd frontend
npm test
npm run build
npm run lint
```

