# Issue tracker: GitHub

As issues e especificações para este repositório utilizam o GitHub Issues. Operações locais de leitura e consulta usam o GitHub CLI (`gh`).

> ⚠️ **Aviso de Restrição:** Em conformidade com as diretrizes do projeto, não execute comandos que criem, alterem ou publiquem issues, labels ou branches remotas sem autorização explícita.

## Convenções de Consulta e Operação

- **Consultar issue:** `gh issue view <number> --json number,title,body,labels,comments`
- **Listar issues abertas:** `gh issue list --state open --json number,title,body,labels,comments`
- **PRs como superfície de requisição:** Desabilitado por padrão (`no`).

## Comportamento das Skills

- Quando uma skill solicitar consultar o ticket relevante, execute a consulta via `gh issue view`.
- Quando uma skill sugerir publicar ou criar uma issue, documente a proposta localmente no plano ou peça autorização antes de disparar `gh issue create`.

