# Storie-12: Integrar SonarCloud com Cobertura de Código via GitHub Actions

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do projeto, quero integrar o SonarCloud com coleta de cobertura de código (OpenCover) no pipeline de CI/CD via GitHub Actions, para garantir análise estática automática, visibilidade de Quality Gate em Pull Requests e bloqueio de merge quando o código não atende os critérios de qualidade mínimos.

## Objetivo
Entregar pipeline CI/CD com análise SonarCloud ativa em PRs para `main`: job `sonar-analysis` executando antes do deploy, coleta de cobertura OpenCover via `coverlet.msbuild`, Quality Gate reportado no PR, badge de qualidade no README e Branch Protection Rule impedindo merge sem o check passar.

## Escopo Técnico
- **Tecnologias:** .NET 10, GitHub Actions, SonarCloud, coverlet (OpenCover), dotnet-sonarscanner
- **Arquivos afetados:**
  - `.github/workflows/deploy-lambda.yml` — adicionar trigger `pull_request`, job `sonar-analysis`, ajustar `needs` do job de deploy
  - `tests/VideoProcessor.Tests.Unit/VideoProcessor.Tests.Unit.csproj` — adicionar `coverlet.msbuild`
  - `tests/VideoProcessor.Tests.Bdd/VideoProcessor.Tests.Bdd.csproj` — adicionar `coverlet.msbuild`
  - `.gitignore` — entradas Sonar/SonarQube
  - `README.md` — badges Quality Gate e Coverage
- **Componentes/Recursos:** job `sonar-analysis`, GitHub Secrets/Variables, SonarCloud project, Branch Protection Rule
- **Pacotes/Dependências:**
  - `coverlet.collector` 6.0.4 (já presente no projeto Unit; verificar BDD)
  - `coverlet.msbuild` 6.0.4 (adicionar em ambos os projetos de teste)
  - `dotnet-sonarscanner` (global tool, instalado no runner via `dotnet tool install --global`)

## Dependências e Riscos (para estimativa)
- **Dependências:** acesso de administrador ao repositório GitHub (para Secrets, Variables e Branch Protection); conta SonarCloud com organização criada; projeto SonarCloud criado antes de executar o pipeline pela primeira vez
- **Riscos:**
  - Automatic Analysis ativa no SonarCloud causa falha do pipeline com exit code 1 — desativar obrigatoriamente antes de rodar CI
  - `sonar.projectBaseDir="."` resolve para `.sonarqube/` no runner — usar `${{ github.workspace }}` (caminho absoluto)
  - PR mostrará "0.0% Coverage on New Code" se os arquivos novos não forem C# (ex.: README) — comportamento esperado, não indica erro
  - Primeira análise da branch `main` requer push/merge para popular o Overview do SonarCloud

## Subtasks
- [x] [Subtask 01: Adicionar coverlet.msbuild aos projetos de teste](./subtask/Subtask-01-Adicionar_Coverlet_MSBuild.md)
- [x] [Subtask 02: Criar job sonar-analysis no workflow](./subtask/Subtask-02-Job_Sonar_Analysis_Workflow.md)
- [x] [Subtask 03: Ajustar trigger do workflow para Pull Requests em main](./subtask/Subtask-03-Trigger_PR_Main_Workflow.md)
- [x] [Subtask 04: Configurar Secrets, Variables no GitHub e projeto no SonarCloud](./subtask/Subtask-04-GitHub_Secrets_SonarCloud_Project.md)
- [x] [Subtask 05: Atualizar .gitignore com entradas do Sonar](./subtask/Subtask-05-Gitignore_Sonar.md)
- [x] [Subtask 06: Branch Protection Rule e badges no README](./subtask/Subtask-06-Branch_Protection_Badges_README.md)

## Critérios de Aceite da História
- [x] `coverlet.msbuild` 6.0.4 adicionado com `PrivateAssets=all` e `IncludeAssets` corretos em ambos os projetos de teste (Unit e BDD)
- [x] Job `sonar-analysis` presente no workflow com `fetch-depth: 0`, `sonar.projectBaseDir="${{ github.workspace }}"` e `sonar.cs.opencover.reportsPaths` apontando para `tests/**/TestResults/**/coverage.opencover.xml`
- [x] Workflow dispara em `pull_request` com alvo `main` (além do `push` em `main` existente); job `deploy` executa **apenas** em push para `main` (não em PR)
- [x] Job `deploy` declara `needs: [sonar-analysis]` e mantém `needs: [build-and-test]` ou equivalente
- [ ] Secret `SONAR_TOKEN` e variables `SONAR_PROJECT_KEY` e `SONAR_ORGANIZATION` criados no GitHub (Settings → Secrets and variables → Actions) — *manual, ver [docs/sonarcloud-setup.md](../../docs/sonarcloud-setup.md)*
- [ ] Projeto criado no SonarCloud e Automatic Analysis desativada (Administration → Analysis Method) — *manual, ver docs/sonarcloud-setup.md*
- [x] `.gitignore` atualizado com entradas `.sonarqube/`, `.scannerwork/` e `coverage.opencover.xml`
- [ ] Branch Protection Rule em `main` exigindo o check `SonarCloud Analysis` como obrigatório antes do merge — *manual, ver docs/sonarcloud-setup.md*
- [x] Badges de Quality Gate e Coverage adicionados no `README.md`
- [ ] Pipeline executa com sucesso em um PR de teste para `main` e Quality Gate é reportado no PR — *após configurar SonarCloud e Secrets*

## Rastreamento (dev tracking)
- **Início:** 13/03/2026, às 12:51 (Brasília)
- **Fim:** —
- **Tempo total de desenvolvimento:** —
