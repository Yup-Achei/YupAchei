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
- **Foco:** Orquestração de casos de uso e serviços de aplicação (ex.: `CatalogoService`, `RecomendacaoService`), mapeamentos de DTOs e regras de conversão.
- **Isolamento e Dublês:** Adotar **fakes leves em memória** implementando as interfaces de repositório (`ILugarRepository`, `IEventoRepository`, `IUserRepository`, etc.) baseados em coleções simples (`List<T>`).
- **Política de Dependências:** Não adicionar bibliotecas externas de mocking (como Moq ou NSubstitute) por padrão, mantendo os testes simples, fáceis de refatorar e sem acoplamento a métodos de configuração de mock.
- **Reutilização:** Reutilizar fakes existentes de `Infrastructure` (ex.: `InMemoryLugarRepository`) apenas quando não houver acoplamento indevido ou necessidade de controle fino de estado no teste; preferir fakes específicos no próprio projeto de teste quando o cenário exigir simulação de falhas ou isolamento estrito.

### 2.3 Testes de Integração (`YupAchei.IntegrationTests`)
- **Foco:** Roteamento de endpoints Minimal API, serialização/deserialização JSON, tratamento global de erros (`DomainException` mapeado para `400 ProblemDetails`, `500` para erros não tratados), validação de cache de saída (`OutputCache` na política `CatalogoPublico`) e códigos de resposta HTTP (`200 OK`, `400 BadRequest`, `404 NotFound`).
- **Infraestrutura e Ferramentas:** Utilizar `WebApplicationFactory<Program>` (do pacote `Microsoft.AspNetCore.Mvc.Testing`).
- **Persistência Sem Dependências Externas:** A suíte deve rodar diretamente contra a infraestrutura em memória e o catálogo seedado (`InMemoryCatalogoSeeder`), sem exigir Docker, containers ou bancos de dados externos.

**Execução:**
```bash
dotnet test backend/YupAchei.sln
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
