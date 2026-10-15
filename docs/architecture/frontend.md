# Arquitetura do Frontend — YupAchei

O frontend do YupAchei é construído com **React 19**, **TypeScript** e **Vite**, adotando uma arquitetura modular orientada a funcionalidades (Feature-driven).

---

## 1. Organização dos Diretórios (`src/`)

- **`app/`:** Ponto de entrada da configuração do aplicativo. Agrupa provedores de contexto globais, roteamento principal e configurações de estado.
- **`features/`:** Módulos funcionais autocontidos do domínio da aplicação. Cada funcionalidade encapsula suas telas, subcomponentes locais, hooks específicos e chamadas de serviço dedicadas.
- **`components/`:** Biblioteca de componentes de interface genéricos e compartilhados (design system, botões, modais, formulários, alertas, tabelas).
- **`services/`:** Camada de infraestrutura de rede. Centraliza as chamadas HTTP (ex.: `fetch` ou `axios`), URLs base, interceptors de autenticação e tratamento de respostas de erro da API.
- **`hooks/`:** Hooks customizados transversais (ex.: `useDebounce`, `useMediaQuery`, `useLocalStorage`).
- **`types/`:** Definições globais de tipos e interfaces em TypeScript, espelhando os contratos e DTOs expostos pelo backend.

---

## 2. Tipagem e Integração com a API

- **Contratos Tipados:** Todas as requisições enviadas e respostas recebidas devem possuir tipos fortemente definidos.
- **Isolamento de Chamadas:** Componentes visuais não realizam chamadas `fetch` diretamente em `useEffect`; consomem funções organizadas na camada `services/` ou hooks especializados.
- **Validação e Resiliência:** Tratamento explícito de estados de carregamento (loading), sucesso e falha (error handling amigável ao usuário).

