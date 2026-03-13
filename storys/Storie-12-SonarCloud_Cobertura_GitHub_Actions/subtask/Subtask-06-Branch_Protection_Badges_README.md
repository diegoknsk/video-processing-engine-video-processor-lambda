# Subtask-06: Branch Protection Rule e Badges no README

## Descrição
Configurar a Branch Protection Rule na branch `main` do GitHub para exigir o check `SonarCloud Analysis` como obrigatório antes do merge, e adicionar os badges de Quality Gate e Coverage no `README.md` do projeto.

## Passos de Implementação
1. **Configurar Branch Protection Rule no GitHub:**
   - Acessar o repositório → **Settings → Branches → Branch protection rules → Add rule**
   - Branch name pattern: `main`
   - Habilitar: **Require status checks to pass before merging**
   - Pesquisar e adicionar o check: `SonarCloud Analysis` (nome exato conforme o campo `name:` do job no workflow)
   - Habilitar: **Require branches to be up to date before merging** (recomendado)
   - Salvar a regra

   ⚠️ O check `SonarCloud Analysis` só aparece na lista de checks disponíveis após o workflow ter executado pelo menos uma vez. Executar o workflow primeiro (via `workflow_dispatch` ou PR de teste) antes de configurar a regra.

2. **Ativar webhook do SonarCloud para PR decoration:**
   - No SonarCloud → projeto → **Project Settings → GitHub**
   - Confirmar que a integração com GitHub está ativa para que o Quality Gate seja reportado diretamente no PR como check de status

3. **Obter URLs dos badges:**
   - No SonarCloud → projeto → Overview → clicar nos badges individuais ou construir manualmente:
     - Quality Gate: `https://sonarcloud.io/api/project_badges/measure?project=<SONAR_PROJECT_KEY>&metric=alert_status`
     - Coverage: `https://sonarcloud.io/api/project_badges/measure?project=<SONAR_PROJECT_KEY>&metric=coverage`

4. **Adicionar badges no README.md:**
   ```markdown
   [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=<SONAR_PROJECT_KEY>&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=<SONAR_PROJECT_KEY>)
   [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=<SONAR_PROJECT_KEY>&metric=coverage)](https://sonarcloud.io/summary/new_code?id=<SONAR_PROJECT_KEY>)
   ```
   Substituir `<SONAR_PROJECT_KEY>` pelo valor real. Posicionar no topo do README, junto aos demais badges se existirem.

## Formas de Teste
1. **Teste de bloqueio de merge:** abrir um PR para `main` com código que reprovaria o Quality Gate (ex.: code smell óbvio) e confirmar que o merge fica bloqueado até o check `SonarCloud Analysis` passar.
2. **Verificação dos badges:** acessar o `README.md` renderizado no GitHub e confirmar que os badges de Quality Gate e Coverage são exibidos com os valores corretos (não aparece "error" ou "inaccessible").
3. **Validação manual da regra:** acessar Settings → Branches e confirmar que a regra está ativa com o check `SonarCloud Analysis` listado.

## Critérios de Aceite
- [ ] Branch Protection Rule criada em `main` com check `SonarCloud Analysis` obrigatório — *manual, ver docs/sonarcloud-setup.md*
- [ ] PR com Quality Gate reprovado fica bloqueado para merge (merge button desabilitado) — *após configurar a regra*
- [x] Badge Quality Gate no README.md apontando para o projeto correto no SonarCloud (placeholder `SONAR_PROJECT_KEY`; substituir após criar projeto)
- [x] Badge Coverage no README.md apontando para o projeto correto no SonarCloud
- [ ] Badges renderizam corretamente no GitHub (status visível, não "error") — *após substituir SONAR_PROJECT_KEY no README*
