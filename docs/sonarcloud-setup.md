# Configuração SonarCloud e GitHub Actions

Este documento descreve os passos manuais para integrar o SonarCloud ao pipeline de CI/CD (Story 12). O workflow já está configurado; é necessário criar o projeto no SonarCloud, definir Secrets/Variables no GitHub e (opcional) a Branch Protection Rule.

## 1. Criar projeto no SonarCloud

1. Acesse [sonarcloud.io](https://sonarcloud.io) e faça login com sua conta GitHub.
2. Na organização desejada, clique em **+** → **Analyze new project**.
3. Selecione o repositório GitHub do projeto.
4. Anote os valores exibidos:
   - **Project Key** (ex.: `fiap_video-processor`)
   - **Organization Key** (slug da organização, ex.: `fiap-org`)

## 2. Desativar Automatic Analysis (obrigatório)

Se a **Automatic Analysis** estiver ativa, o pipeline falhará com:

```text
You are running CI analysis while Automatic Analysis is enabled
```

1. No SonarCloud → projeto → **Administration** → **Analysis Method**.
2. Desative o toggle **Automatic Analysis**.

## 3. Gerar SONAR_TOKEN

1. Acesse [sonarcloud.io/account/security](https://sonarcloud.io/account/security).
2. Crie um novo token (tipo **Project Analysis Token** para o projeto ou **Global Analysis Token**).
3. Copie o valor — ele só é exibido uma vez.

## 4. Configurar GitHub (Secret)

1. No repositório GitHub → **Settings** → **Secrets and variables** → **Actions**.

2. **Secrets** → **New repository secret**:
   - Nome: `SONAR_TOKEN`
   - Valor: token gerado no passo 3

O workflow usa projeto e organização fixos (`diegoknsk_video-processing-engine-video-processor-lambda` e `diegoknsk`); não é necessário criar variables no GitHub para o SonarCloud.

## 5. Projeto duplicado (repositório público)

Se o repositório for público, o SonarCloud pode ter criado um projeto automaticamente. Se existir mais de um projeto para o mesmo repositório:

- Escolha um projeto (geralmente o vinculado ao repo).
- Desative a Automatic Analysis nele.
- No projeto duplicado → **Administration** → **Deletion** para removê-lo.

## 6. Badges no README

Os badges de Quality Gate e Coverage no topo do `README.md` já apontam para o projeto `diegoknsk_video-processing-engine-video-processor-lambda` e passam a exibir o status após a primeira análise no SonarCloud.

## 7. Branch Protection Rule (opcional, recomendado)

Para exigir que o check do SonarCloud passe antes do merge em `main`:

1. **Settings** → **Branches** → **Branch protection rules** → **Add rule**.
2. Branch name pattern: `main`.
3. Marque **Require status checks to pass before merging**.
4. Em "Status checks that are required", pesquise e adicione: **SonarCloud Analysis** (nome exato do job no workflow).
5. (Recomendado) Marque **Require branches to be up to date before merging**.
6. Salve a regra.

> O check **SonarCloud Analysis** só aparece na lista após o workflow ter sido executado pelo menos uma vez (por exemplo via `workflow_dispatch` ou um PR para `main`).

## 8. Webhook SonarCloud → GitHub (decoração de PR)

Para o Quality Gate aparecer diretamente no PR como status check:

1. No SonarCloud → projeto → **Project Settings** → **GitHub**.
2. Confirme que a integração com GitHub está ativa.

## Resumo do que o pipeline espera

| Tipo   | Nome          | Onde   | Descrição |
|--------|---------------|--------|-----------|
| Secret | `SONAR_TOKEN` | GitHub | Token do SonarCloud em [account/security](https://sonarcloud.io/account/security) |

Projeto e organização estão fixos no workflow: `diegoknsk_video-processing-engine-video-processor-lambda` e `diegoknsk`. Com o secret configurado, o job **SonarCloud Analysis** roda em push e em Pull Requests para `main`, e o deploy só ocorre após o Quality Gate (e o build-and-test) passarem.
