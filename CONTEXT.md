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
  - Estilo: Tailwind CSS 4, com tokens de marca em `frontend/src/index.css`.
  - Testes: Vitest + Testing Library.
  - Tipagem: TypeScript em modo estrito (`strict: true`).
- **Orquestração Local:**
  - Docker Compose para empacotamento e orquestração dos serviços de desenvolvimento (`backend` e `frontend`).

---

## 3. Estado das Definições de Negócio

- **Domínio:** Plataforma de descoberta e curadoria urbana local (iniciando por Porto Velho, RO).
- **Requisitos Funcionais Confirmados:**
  - **RF01 — Catálogo e Visualização de Lugares:** Listagem e detalhe de locais (`Place`) nas categorias do protótipo (`Cafeterias`, `Restaurantes`, `Bares`, `Cultura`, `Passeios`, `Esportes`), contendo descrição, fotos, endereço, bairro, região urbana, coordenadas, links úteis (Google Maps, Instagram, WhatsApp), horário de funcionamento e nota média.
  - **RF02 — Agenda de Eventos:** Eventos temporários (`Event`) com início/fim, faixa de preço, link externo de ingressos e localização flexível (vinculada a um `Place` cadastrado **ou** endereço avulso temporário).
  - **RF03 — Curadoria de Parceiros e Influenciadores:** Depoimentos assinados (`PartnerReview`) em destaque nos cards dos lugares (avatar, nome, rede social, nota/selo e comentário curto).
  - **RF04 — Busca e Filtros Determinísticos:** Filtros por categoria, região urbana/bairro, etiquetas de ambiente (`VibeTag`), disponibilidade no momento (`Aberto agora`) e nota mínima (`4.5+`).
  - **RF05 — Recomendações Inteligentes (Yupy AI):** Intenção em texto livre retornando de 3 a 5 sugestões contextualizadas do catálogo ativo.
  - **RF06 — Autenticação e Interação do Usuário:** Login para usuários avaliarem estabelecimentos (`UserReview`) e gerenciarem lugares salvos (`SavedPlace`), mantendo toda a leitura pública sem fricção.
- **Requisitos Não-Funcionais Confirmados:**
  - **RNF01 — Leitura Pública Zero Friction:** Todos os endpoints de consulta do catálogo, eventos e busca são 100% públicos; autenticação é exigida apenas para ações do usuário (avaliar, salvar na conta).
  - **RNF02 — Desempenho e Latência:** Listagens públicas com tempo de resposta < 200ms via cache em memória/HTTP.
  - **RNF03 — Resiliência do Recomendador (Fallback):** Em caso de falha/timeout/cota da LLM, retornar recomendações determinísticas baseadas em popularidade/curadoria.
  - **RNF04 — Otimização de Mídia:** URLs apontando para CDNs otimizadas / WebP.
- **Vocabulário Ubíquo:** Documentado em [GLOSSARY.md](GLOSSARY.md).

