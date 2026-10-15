# Contexto Técnico do Projeto — YupAchei

Este documento consolida o estado técnico e as premissas confirmadas do projeto **YupAchei**.

---

## 1. Identificação Geral

- **Nome do Projeto:** YupAchei
- **Repositório de Referência:** `https://github.com/CunhaRo/YupAchei`
- **Modelo de Desenvolvimento:** Desenvolvimento assistido por agentes de inteligência artificial em conjunto com múltiplos desenvolvedores humanos.

---

## 2. Tecnologias Confirmadas

- **Backend:**
  - Plataforma: .NET 10 (C# 14).
  - Padrão Arquitetural: Clean Architecture dividida em 4 camadas (`Domain`, `Application`, `Infrastructure`, `API`).
  - Framework de Testes: xUnit.
- **Frontend:**
  - Framework: React 19 + TypeScript.
  - Ferramenta de Build: Vite.
  - Tipagem: TypeScript em modo estrito (`strict: true`).
- **Orquestração Local:**
  - Docker Compose para empacotamento e orquestração dos serviços de desenvolvimento (`backend` e `frontend`).

---

## 3. Estado das Definições de Negócio

- **Público-alvo:** *[Pendente de definição formal]*
- **Requisitos Funcionais:** *[Pendente de definição formal]*
- **Entidades de Domínio:** *[Pendente de definição formal]*
- **Regras de Negócio e Casos de Uso:** *[Pendente de definição formal]*
- **Persistência / Banco de Dados Específico:** *[Pendente de especificação de requisitos]*
- **Integrações Externas (ex.: Serviços de IA/LLM):** *[Pendente de especificação de requisitos]*

> **Aviso para agentes:** Não crie entidades ou regras de negócio hipotéticas ou fictícias. Toda modelagem de domínio deve aguardar o levantamento de requisitos com as partes interessadas.

