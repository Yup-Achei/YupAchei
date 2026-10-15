# Regras e Limites Arquiteturais Compartilhados

Este documento formaliza as fronteiras arquiteturais invioláveis do projeto **YupAchei**.

---

## 1. Fronteiras entre Backend e Frontend

- O **Backend** (.NET 10) e o **Frontend** (React + TypeScript) são sistemas autônomos.
- A única forma de comunicação permitida é via protocolo HTTP/REST através de endpoints expostos pelo projeto `YupAchei.API`.
- O frontend não acessa diretamente banco de dados, variáveis de ambiente do backend ou implementações de infraestrutura.
- Todos os contratos de dados trocados entre frontend e backend devem ser tipados em ambas as pontas (DTOs no C# e types/interfaces no TypeScript).

---

## 2. Limites Internos do Backend (Clean Architecture)

1. **Inversão de Dependência:** As dependências devem sempre apontar para dentro, em direção ao núcleo do domínio.
   - `Domain` não conhece nenhuma outra camada.
   - `Application` conhece apenas `Domain`.
   - `Infrastructure` conhece `Domain` e `Application` para implementar contratos e persistência.
   - `API` orquestra e compõe as dependências, conhecendo `Application` e `Infrastructure`.
2. **Proibição de Atalhos:**
   - A camada `API` jamais deve acessar repositórios ou o `DbContext` diretamente sem passar pela camada `Application`.
   - O `DbContext` nunca deve ser injetado fora da camada `Infrastructure`.
   - Entidades de domínio não devem vazar diretamente para o frontend; use sempre DTOs.
3. **Exceções e ADRs:** Qualquer necessidade de desvio ou quebra temporária deste padrão arquitetural exige uma Arquitetura Decision Record (ADR) aprovada e registrada em `docs/architecture/decisions/`.

---

## 3. Gestão de Estado e Efeitos Colaterais

- Métodos de leitura (queries) não devem produzir efeitos colaterais nos dados.
- Operações de escrita (commands) devem validar consistência e invariantes de negócio na camada de domínio e aplicação antes de persistir dados.

