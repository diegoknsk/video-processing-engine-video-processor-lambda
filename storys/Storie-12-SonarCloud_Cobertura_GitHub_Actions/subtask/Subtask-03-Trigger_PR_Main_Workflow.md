# Subtask-03: Ajustar Trigger do Workflow para Pull Requests em main

## Descrição
Atualizar a seção `on:` do arquivo `.github/workflows/deploy-lambda.yml` para que o workflow também seja disparado em Pull Requests com alvo `main`. Com esse ajuste, o job `sonar-analysis` (e o `build-and-test`) executará em PRs, entregando feedback de qualidade antes do merge, enquanto o job `deploy` permanece restrito a push em `main`.

## Passos de Implementação
1. Abrir `.github/workflows/deploy-lambda.yml` e localizar a seção `on:` atual:
   ```yaml
   on:
     push:
       branches: [main]
     workflow_dispatch:
   ```

2. Adicionar o trigger `pull_request`:
   ```yaml
   on:
     push:
       branches: [main]
     pull_request:
       branches: [main]
     workflow_dispatch:
   ```

3. Confirmar que o job `deploy` possui `if: github.ref == 'refs/heads/main'` para garantir que **nunca** executa em eventos de `pull_request`. O contexto `github.ref` em PRs é `refs/pull/<number>/merge`, portanto a condição existente já protege o deploy — validar que está presente.

4. Confirmar que o job `sonar-analysis` **não** possui restrição `if:` de branch, para que rode tanto em push para `main` quanto em PRs.

## Formas de Teste
1. **Teste em PR:** criar um branch de teste, abrir PR para `main` e confirmar que os jobs `build-and-test` e `sonar-analysis` são disparados, mas o job `deploy` não aparece ou fica skipped.
2. **Teste em push direto:** fazer push para `main` (ou merge do PR) e confirmar que todos os jobs executam, incluindo `deploy`.
3. **Verificação no GitHub Actions UI:** na aba Actions do repositório, ao clicar em uma execução disparada por PR, verificar que o job `deploy` está com status "skipped" ou não presente.

## Critérios de Aceite
- [x] Seção `on:` inclui `pull_request: branches: [main]`
- [x] Jobs `build-and-test` e `sonar-analysis` executam em eventos de PR para `main`
- [x] Job `deploy` não executa em eventos de PR (protegido por `if: github.ref == 'refs/heads/main'`)
- [x] `workflow_dispatch` mantido para disparos manuais
