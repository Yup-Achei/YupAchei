# Diretrizes para Agentes de Código e Desenvolvedores — YupAchei

Este documento define as regras permanentes e obrigatórias de engenharia para qualquer agente de inteligência artificial ou desenvolvedor que atue no repositório **YupAchei**.

---

## 1. Princípios Fundamentais

1. **Consulta Prévia Obrigatória:** Consulte sempre as instruções (`AGENTS.md`), o contexto técnico (`CONTEXT.md`) e a documentação em `docs/` antes de propor ou implementar qualquer alteração.
2. **Respeito à Clean Architecture:** Siga rigorosamente a separação de responsabilidades e as regras de dependência entre as camadas do backend. Não contorne camadas sem justificativa documentada e aprovada.
3. **Isolamento entre Backend e Frontend:** O backend (.NET 10) e o frontend (React + TypeScript) são aplicações independentes, comunicando-se exclusivamente via contratos HTTP/REST.
4. **Planejamento com Critérios de Aceitação:** Toda tarefa técnica ou funcional deve iniciar com um plano claro, detalhando etapas, impactos e critérios de aceitação verificáveis.
5. **Utilização de Skills:** Empregue as skills versionadas em `.agents/skills/` conforme aplicável a cada etapa (modelagem de domínio, TDD, revisão de código, diagnóstico de bugs).
6. **Testes Obrigatórios e Reais:** Escreva e execute testes unitários e de integração pertinentes para qualquer código novo ou alterado. **Nunca declare testes executados sem tê-los rodado de fato.**
7. **Revisão de Diffs:** Inspecione detalhadamente o diff (`git diff`) antes de concluir qualquer tarefa para garantir que nenhum arquivo indesejado, temporário ou alteração inadvertida seja introduzida.
8. **Parcimônia de Dependências:** Não introduza bibliotecas ou pacotes de terceiros sem justificativa técnica evidente.
9. **Sigilo e Segurança:** Nunca exponha credenciais, chaves de API, senhas, tokens ou dados sensíveis no código ou no histórico.
10. **Ações Não Destrutivas:** Operações destrutivas, exclusão de histórico, sobrescrita de arquivos pré-existentes ou alteração de configurações globais do ambiente exigem confirmação explícita.
11. **Registro de Decisões:** Registre decisões arquiteturais relevantes (ADRs) em `docs/architecture/decisions/`.
12. **Transparência na Entrega:** Ao finalizar qualquer etapa, apresente com clareza o que foi alterado, quais testes foram executados e seus resultados, e quais pendências restam abertas.

---

## 2. Navegação Rápida

- **Contexto Geral:** [CONTEXT.md](CONTEXT.md)
- **Regras Arquiteturais Compartilhadas:** [.agents/rules/architecture.md](.agents/rules/architecture.md)
- **Diretrizes do Backend:** [backend/AGENTS.md](backend/AGENTS.md)
- **Diretrizes do Frontend:** [frontend/AGENTS.md](frontend/AGENTS.md)
- **Documentação de Arquitetura:** [docs/architecture/overview.md](docs/architecture/overview.md)
- **Fluxo de Desenvolvimento:** [docs/development/workflow.md](docs/development/workflow.md)
- **Estratégia de Testes:** [docs/development/testing.md](docs/development/testing.md)



---

## 3. Agent Skills

### Issue tracker
GitHub Issues (consulta local via \gh\). Veja [\docs/agents/issue-tracker.md\](docs/agents/issue-tracker.md).

### Triage labels
Rótulos canônicos de triagem (\
eeds-triage\, \
eeds-info\, \eady-for-agent\, \eady-for-human\, \wontfix\). Veja [\docs/agents/triage-labels.md\](docs/agents/triage-labels.md).

### Domain docs e Arquitetura
Contexto em [\CONTEXT.md\](CONTEXT.md), arquitetura em [\docs/architecture/\](docs/architecture/) e ADRs em [\docs/architecture/decisions/\](docs/architecture/decisions/). Veja [\docs/agents/domain.md\](docs/agents/domain.md).

