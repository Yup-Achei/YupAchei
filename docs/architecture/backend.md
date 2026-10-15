# Arquitetura do Backend — YupAchei

O backend do YupAchei é implementado em **.NET 10** (C# 14), estruturado sob os princípios da **Clean Architecture**.

---

## 1. Camadas da Solução

### 1.1 `YupAchei.Domain` (Núcleo)
- **Papel:** Contém a essência das regras de negócio do sistema.
- **Componentes:** Entidades, Objetos de Valor (Value Objects), Agregados, Enums, Eventos de Domínio e Interfaces de Repositório.
- **Regra de Dependência:** Isolamento total. Não referencia nenhum outro projeto ou pacote externo que não seja padrão do .NET.

### 1.2 `YupAchei.Application` (Casos de Uso)
- **Papel:** Orquestra a execução das regras de negócio, mediando a entrada de dados e as operações de domínio.
- **Componentes:** Casos de uso / Handlers, DTOs (Data Transfer Objects), Mapeamentos, Validadores e Interfaces de serviços externos.
- **Regra de Dependência:** Referencia exclusivamente `YupAchei.Domain`.

### 1.3 `YupAchei.Infrastructure` (Implementações Técnicas)
- **Papel:** Fornece as implementações concretas dos contratos definidos em `Domain` e `Application`.
- **Componentes:** DbContext, migrações de banco, implementações de repositórios, clientes de serviços externos e provedores de mensageria.
- **Regra de Dependência:** Referencia `YupAchei.Domain` e `YupAchei.Application`.

### 1.4 `YupAchei.API` (Entrada e Composição)
- **Papel:** Ponto de entrada HTTP do sistema e raiz de composição de dependências (Composition Root).
- **Componentes:** Endpoints (Minimal APIs / Controllers), Middlewares, Tratamento global de exceções, Configuração de Injeção de Dependências e Documentação OpenAPI.
- **Regra de Dependência:** Referencia `YupAchei.Application` e `YupAchei.Infrastructure`.

---

## 2. Estratégia de Testes

- **`YupAchei.Domain.Tests`:** Testes unitários focados nas invariantes e lógicas puras de entidades e value objects.
- **`YupAchei.Application.Tests`:** Testes unitários para validar a orquestração dos casos de uso, cobrindo fluxos felizes e exceções de negócio, com dublês de teste para interfaces.
- **`YupAchei.IntegrationTests`:** Testes integrados ponta a ponta utilizando `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`), assegurando o correto roteamento, serialização e persistência.

