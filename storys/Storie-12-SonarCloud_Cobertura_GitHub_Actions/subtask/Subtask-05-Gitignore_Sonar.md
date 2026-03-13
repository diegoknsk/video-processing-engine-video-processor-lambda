# Subtask-05: Atualizar .gitignore com Entradas do Sonar

## Descrição
Adicionar ao `.gitignore` do repositório as entradas necessárias para evitar que arquivos temporários gerados pelo SonarScanner e pela coleta de cobertura sejam versionados acidentalmente.

## Passos de Implementação
1. Localizar o arquivo `.gitignore` na raiz do repositório.

2. Verificar se já existem entradas relacionadas ao Sonar (buscar por `.sonarqube`, `.scannerwork`). Se não existirem, adicionar a seção:
   ```gitignore
   # SonarCloud / SonarQube
   .sonarqube/
   **/.sonarqube/
   **/out/.sonar/
   .scannerwork/
   **/.scannerwork/
   coverage.opencover.xml
   ```

3. Verificar se já existe entrada para `TestResults/` — se não existir, adicionar também:
   ```gitignore
   # Test results and coverage
   **/TestResults/
   ```
   (Caso já exista, não duplicar.)

4. Salvar o arquivo e executar `git status` para confirmar que nenhum arquivo `.sonarqube/` ou `coverage.opencover.xml` existente passa a aparecer como "untracked" que deveria estar ignorado.

## Formas de Teste
1. **git check-ignore:** executar `git check-ignore -v .sonarqube/` e `git check-ignore -v coverage.opencover.xml` — ambos devem retornar a regra do `.gitignore` que os ignora.
2. **Após rodar o scanner localmente:** executar o SonarScanner localmente (ver `reference.md` da skill) e confirmar que `git status` não mostra `.sonarqube/` como arquivo não rastreado.
3. **Revisão manual:** abrir o `.gitignore` e confirmar que todas as 5 entradas da seção Sonar estão presentes.

## Critérios de Aceite
- [x] Entrada `.sonarqube/` presente no `.gitignore`
- [x] Entrada `.scannerwork/` presente no `.gitignore`
- [x] Entrada `coverage.opencover.xml` presente no `.gitignore`
- [x] `git check-ignore -v .sonarqube/` retorna a regra correspondente (arquivo corretamente ignorado)
