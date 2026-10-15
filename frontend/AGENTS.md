# Diretrizes para o Frontend (React + TypeScript / Vite)

Este documento complementa o [AGENTS.md](../AGENTS.md) da raiz com diretrizes específicas para o frontend do **YupAchei**.

---

## 1. Estrutura de Diretórios e Responsabilidades

A estrutura modular em `frontend/src/` organiza a aplicação de forma desacoplada:

- `src/app/`: Configurações globais da aplicação (provedores de contexto, roteador, estado global).
- `src/features/`: Módulos de domínio funcional da aplicação. Cada feature pode conter seus próprios componentes, hooks e tipos específicos.
- `src/components/`: Componentes reutilizáveis compartilhados e genéricos (botões, modais, layouts, inputs).
- `src/services/`: Clientes e funções de comunicação HTTP com o backend YupAchei.API, centralizando URLs e tratamento base de requisições.
- `src/hooks/`: Hooks customizados utilitários compartilhados.
- `src/types/`: Definições globais de tipos e contratos de dados sincronizados com a API backend.

---

## 2. Padrões de TypeScript e Tipagem Estrita

1. **Strict Mode Obrigatório:** O compilador está configurado em modo estrito (`strict: true`).
2. **Proibição de `any` implícito ou explícito:** Utilize tipos específicos, `unknown` (com type narrowing defensivo) ou tipos genéricos adequados.
3. **Tipagem de Respostas de API:** Todas as chamadas em `services/` devem tipar explicitamente os retornos com base nos contratos em `types/`.
4. **Props de Componentes:** Declare explicitamente interfaces para props de componentes (`interface ComponentProps { ... }`).
5. **Tratamento de Erros:** Não ignore falhas de rede ou respostas com status de erro. Encapsule erros em estruturas amigáveis para a interface do usuário.

---

## 3. Qualidade e Comandos de Validação

Antes de concluir qualquer alteração no frontend:

```bash
# Validação de tipos e compilação
npm run build

# Validação com linter
npm run lint
```

Consulte [docs/architecture/frontend.md](../docs/architecture/frontend.md) para detalhes arquiteturais completos.

