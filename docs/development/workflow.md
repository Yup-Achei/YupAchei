# Fluxo de Desenvolvimento — YupAchei

Este documento descreve o fluxo de trabalho padronizado para desenvolvedores humanos e agentes de código no projeto **YupAchei**.

---

## Fases do Fluxo de Trabalho

```
┌──────────────┐     ┌──────────────┐     ┌────────────────┐
│ 1. Demanda   ├────►│ 2. Análise   ├────►│ 3. Planejamento│
└──────────────┘     └──────────────┘     └───────┬────────┘
                                                  │
┌──────────────┐     ┌──────────────┐     ┌───────▼────────┐
│ 6. Revisão   │◄────┤ 5. Testes    │◄────┤ 4. Impl.       │
│    e Diff    │     │    e Validação│    │    (TDD/Clean) │
└──────────────┘     └──────────────┘     └────────────────┘
```

### 1. Demanda e Critérios de Aceitação
- Nenhuma implementação deve ter início sem critérios de aceitação objetivos e mensuráveis.
- Dúvidas sobre escopo ou comportamento devem ser esclarecidas previamente antes da alteração de código.

### 2. Análise e Consulta da Documentação
- Inspecione `CONTEXT.md`, `AGENTS.md` e os documentos pertinentes em `docs/`.
- Verifique se a demanda impacta contratos de dados entre backend e frontend ou viola limites de camadas.

### 3. Planejamento
- Elabore um plano de execução estruturado passo a passo.
- Identifique os arquivos a serem criados ou alterados, os testes a serem adicionados e os impactos em outros módulos.

### 4. Implementação
- Aplique o princípio da responsabilidade única (SRP) e respeite a Clean Architecture.
- Escreva código tipado, sem introduzir dependências ou complexidade acidental.

### 5. Execução de Testes e Validação
- Execute `dotnet test` para backend e `npm run build && npm run lint` para frontend.
- Nunca considere uma tarefa concluída com testes quebrados ou não executados.

### 6. Revisão de Diffs e Documentação da Entrega
- Avalie o resultado do comando `git diff` e `git status`.
- Registre o sumário das alterações realizadas, os testes executados com seus resultados e qualquer pendência identificada.

