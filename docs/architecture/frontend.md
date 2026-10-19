# Arquitetura do Frontend — YupAchei

O frontend do YupAchei é construído com **React 19**, **TypeScript**, **Vite** e **Tailwind CSS 4**, adotando uma arquitetura modular orientada a funcionalidades (Feature-driven). O estilo usa poucas classes próprias. As cores e a fonte da marca são tokens em `src/index.css` (`--color-brand`, `--font-sans`), que geram utilitários como `bg-brand`. Não há `tailwind.config.js`. O alias `@/` aponta para `src/`, declarado em `tsconfig.app.json` (`paths`) e nos aliases de `vite.config.ts` e `vitest.config.ts`. No TypeScript 6, `paths` resolve o alias sem `baseUrl`. Os testes de componentes usam Vitest e Testing Library (`npm test`, a partir de `frontend/`).

---

## 1. Organização dos Diretórios (`src/`)

- **`app/`:** Ponto de entrada da configuração do aplicativo. Agrupa provedores de contexto globais, roteamento principal e configurações de estado.
- **`pages/`:** Uma tela por rota. Compõe as features quando a tela reúne mais de um assunto, como a Home, que mostra categorias, locais e eventos.
- **`features/`:** Módulos funcionais autocontidos do domínio da aplicação. Cada funcionalidade encapsula suas telas, subcomponentes locais, hooks específicos e chamadas de serviço dedicadas.
- **`components/`:** Biblioteca de componentes de interface genéricos e compartilhados (design system, botões, modais, formulários, alertas, tabelas).
- **`mocks/`:** JSON de exemplo de categorias, locais e eventos, usado enquanto as telas ainda não chamam a API.
- **`services/`:** Camada de infraestrutura de rede. Centraliza as chamadas HTTP (ex.: `fetch` ou `axios`), URLs base, interceptors de autenticação e tratamento de respostas de erro da API.
- **`hooks/`:** Hooks customizados transversais (ex.: `useDebounce`, `useMediaQuery`, `useLocalStorage`).
- **`types/`:** Definições globais de tipos e interfaces em TypeScript, espelhando os contratos e DTOs expostos pelo backend.

---

## 2. Tipagem e Integração com a API

- **Contratos Tipados:** Todas as requisições enviadas e respostas recebidas devem possuir tipos fortemente definidos.
- **Isolamento de Chamadas:** Componentes visuais não realizam chamadas `fetch` diretamente em `useEffect`; consomem funções organizadas na camada `services/` ou hooks especializados.
- **Validação e Resiliência:** Tratamento explícito de estados de carregamento (loading), sucesso e falha (error handling amigável ao usuário).

