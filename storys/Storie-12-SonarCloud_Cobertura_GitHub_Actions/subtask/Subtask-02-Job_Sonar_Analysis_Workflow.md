# Subtask-02: Criar Job sonar-analysis no Workflow

## Descrição
Adicionar o job `sonar-analysis` ao arquivo `.github/workflows/deploy-lambda.yml`, posicionado **antes** do job `deploy`, com todas as etapas obrigatórias: checkout com `fetch-depth: 0`, instalação do `dotnet-sonarscanner`, `begin` com `sonar.projectBaseDir` absoluto, build, `dotnet test` com coleta OpenCover e `end`. O job `deploy` deve declarar `needs: [sonar-analysis]`.

## Passos de Implementação
1. Abrir `.github/workflows/deploy-lambda.yml` e adicionar o job `sonar-analysis` entre o job `build-and-test` e o job `deploy`:

   ```yaml
   sonar-analysis:
     name: SonarCloud Analysis
     runs-on: ubuntu-latest
     steps:
       - name: Checkout code
         uses: actions/checkout@v4
         with:
           fetch-depth: 0

       - name: Setup .NET
         uses: actions/setup-dotnet@v4
         with:
           dotnet-version: '10.0.x'

       - name: Install SonarScanner
         run: dotnet tool install --global dotnet-sonarscanner

       - name: Restore dependencies
         run: dotnet restore

       - name: Begin SonarCloud analysis
         run: |
           dotnet sonarscanner begin \
             /k:"${{ vars.SONAR_PROJECT_KEY }}" \
             /o:"${{ vars.SONAR_ORGANIZATION }}" \
             /d:sonar.token="${{ secrets.SONAR_TOKEN }}" \
             /d:sonar.host.url="https://sonarcloud.io" \
             /d:sonar.projectBaseDir="${{ github.workspace }}" \
             /d:sonar.sources="src/" \
             /d:sonar.tests="tests/" \
             /d:sonar.exclusions="**/bin/**,**/obj/**,**/*.Designer.cs" \
             /d:sonar.test.exclusions="tests/**/" \
             /d:sonar.coverage.exclusions="**/Program.cs,**/*Extensions.cs" \
             /d:sonar.cs.opencover.reportsPaths="tests/**/TestResults/**/coverage.opencover.xml"

       - name: Build solution
         run: dotnet build --configuration Release --no-restore

       - name: Run tests with coverage
         run: |
           dotnet test \
             --configuration Release \
             --no-build \
             --verbosity normal \
             /p:CollectCoverage=true \
             /p:CoverageReporter=opencover \
             /p:CoverletOutputFormat=opencover \
             /p:CoverletOutput=./TestResults/coverage.opencover.xml

       - name: End SonarCloud analysis
         run: |
           dotnet sonarscanner end \
             /d:sonar.token="${{ secrets.SONAR_TOKEN }}"
   ```

2. Atualizar o job `deploy` para declarar `needs: [sonar-analysis]` (mantendo também a dependência de `build-and-test` se ainda necessária, ou consolidando em `needs: [build-and-test, sonar-analysis]`).

3. Garantir que o job `sonar-analysis` NÃO tem condição `if: github.ref == 'refs/heads/main'` — ele deve executar em PRs também. O job `deploy` deve manter `if: github.ref == 'refs/heads/main'` para nunca executar em PRs.

## Formas de Teste
1. **Validação de sintaxe YAML:** executar `yamllint .github/workflows/deploy-lambda.yml` ou usar a validação do GitHub Actions no repositório (tab Actions → verificar se o workflow é parseado sem erros de sintaxe).
2. **Execução em PR de teste:** abrir um PR para `main` e verificar se o job `SonarCloud Analysis` aparece nos checks do PR e conclui com sucesso.
3. **Log do runner:** no log do job, confirmar que o step `End SonarCloud analysis` exibe "ANALYSIS SUCCESSFUL" e o link para o projeto no SonarCloud.

## Critérios de Aceite
- [x] Job `sonar-analysis` presente no workflow com `fetch-depth: 0` no checkout
- [x] `sonar.projectBaseDir` usa `${{ github.workspace }}` (caminho absoluto, nunca `"."`)
- [x] `dotnet test` é executado com flags `/p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./TestResults/coverage.opencover.xml`
- [x] Job `deploy` declara `needs` incluindo `sonar-analysis`
- [x] Job `deploy` mantém `if: github.ref == 'refs/heads/main'` — não executa em PRs
- [x] Workflow YAML válido (sem erros de sintaxe)
