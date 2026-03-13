# Subtask-04: Configurar Secrets/Variables no GitHub e Projeto no SonarCloud

## Descrição
Executar as etapas manuais necessárias para conectar o pipeline ao SonarCloud: criar o projeto no SonarCloud, desativar Automatic Analysis, gerar o token e registrar o Secret `SONAR_TOKEN` e as Variables `SONAR_PROJECT_KEY` e `SONAR_ORGANIZATION` no GitHub.

## Passos de Implementação
1. **Criar projeto no SonarCloud:**
   - Acessar [sonarcloud.io](https://sonarcloud.io) → organização → **+** Analyze new project
   - Selecionar o repositório GitHub do projeto
   - Anotar os valores de **Project Key** e **Organization Key** exibidos

2. **Desativar Automatic Analysis (obrigatório antes de rodar o CI):**
   - No SonarCloud → projeto → **Administration → Analysis Method**
   - Desativar o toggle **Automatic Analysis**
   - ⚠️ Se não desativado, o pipeline falhará com: `You are running CI analysis while Automatic Analysis is enabled`

3. **Gerar SONAR_TOKEN:**
   - Acessar [sonarcloud.io/account/security](https://sonarcloud.io/account/security)
   - Criar novo token (tipo: **Project Analysis Token** para o projeto específico, ou **Global Analysis Token**)
   - Copiar o valor — ele só é exibido uma vez

4. **Registrar no GitHub:**
   - **Secret:** Settings → Secrets and variables → Actions → **New repository secret**
     - Nome: `SONAR_TOKEN` | Valor: token gerado no passo 3
   - **Variable:** Settings → Secrets and variables → Actions → **Variables** → **New repository variable**
     - Nome: `SONAR_PROJECT_KEY` | Valor: Project Key do SonarCloud (ex.: `fiap_video-processor`)
     - Nome: `SONAR_ORGANIZATION` | Valor: Organization slug do SonarCloud (ex.: `fiap-org`)

5. **Verificar se há projeto duplicado:** se o repositório é público, o SonarCloud pode ter criado um projeto automático. Conferir se há dois projetos para o mesmo repo e deletar o órfão via **Administration → Deletion** no projeto duplicado.

## Formas de Teste
1. **Validação manual do SonarCloud:** acessar o projeto no SonarCloud e confirmar que o status de análise mostra "Waiting for CI" (ou similar) após desativar Automatic Analysis.
2. **Pipeline de validação:** executar o workflow (via `workflow_dispatch`) e confirmar que o step `Begin SonarCloud analysis` não retorna erro de autenticação (token inválido retorna 401).
3. **Log de sucesso:** verificar no log do step `End SonarCloud analysis` a mensagem "ANALYSIS SUCCESSFUL" com link para o projeto — confirma que token, project key e organization estão corretos.

## Critérios de Aceite
- [ ] Projeto criado no SonarCloud para o repositório correto — *manual*
- [ ] Automatic Analysis desativada em Administration → Analysis Method — *manual*
- [ ] Secret `SONAR_TOKEN` criado no GitHub (Settings → Secrets → Actions) — *manual*
- [ ] Variable `SONAR_PROJECT_KEY` criada no GitHub (Settings → Variables → Actions) — *manual*
- [ ] Variable `SONAR_ORGANIZATION` criada no GitHub (Settings → Variables → Actions) — *manual*
- [ ] Ausência de projetos duplicados no SonarCloud para o mesmo repositório — *manual*

**Documentação:** passos detalhados em [docs/sonarcloud-setup.md](../../docs/sonarcloud-setup.md).
