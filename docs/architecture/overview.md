# Visão Geral da Arquitetura — YupAchei

Este documento apresenta a arquitetura de alto nível do sistema **YupAchei**, delimitando responsabilidades entre componentes e descrevendo a infraestrutura local.

---

## 1. Visão de Sistema

O YupAchei é concebido com arquitetura desacoplada em duas aplicações autônomas:

```
┌─────────────────────────────────┐
│     Cliente / Usuário Final     │
└────────────────┬────────────────┘
                 │
                 ▼
┌─────────────────────────────────┐
│            Frontend             │
│    (React + Vite + TypeScript)  │
└────────────────┬────────────────┘
                 │ Chamadas REST / JSON
                 ▼
┌─────────────────────────────────┐
│             Backend             │
│    (.NET 10 Clean Architecture) │
└─────────────────────────────────┘
```

---

## 2. Limites entre Aplicações

- **Independência de Ciclo de Vida:** Backend e frontend possuem seus próprios arquivos de configuração, ferramentas de build, linters e suítes de teste.
- **Contratos Explícitos:** A comunicação é estritamente realizada via HTTP através de endpoints documentados (OpenAPI/Swagger).
- **Sem Acoplamento de Tecnologias:** Nenhuma biblioteca ou modelo de classes do C# é diretamente importado no ecossistema Node/TypeScript. Em vez disso, tipos TypeScript equivalentes representam os DTOs trafegados.

---

## 3. Papel do Docker Compose

O arquivo `docker-compose.yml` na raiz serve ao propósito de:
- Padronizar o ambiente de desenvolvimento local para desenvolvedores e agentes.
- Garantir que tanto o backend quanto o frontend possam ser instanciados com suas respectivas dependências em containers isolados.
- Isolar a rede interna através de uma bridge dedicada (`yupachei-net`).

*Nota:* Serviços complementares como bancos de dados relacionais, caches em memória e filas assíncronas serão incorporados ao Docker Compose à medida que requisitos de negócio específicos forem aprovados.

