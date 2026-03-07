# Subtask 02: Deploy produção somente em main; deploy dev em branch dev

## Descrição
Ajustar o workflow de deploy para que: (1) o deploy para **produção** ocorra somente quando o código for enviado para a branch **main**; (2) o deploy para o ambiente de **desenvolvimento** ocorra automaticamente quando houver push na branch **dev**.

## Passos de implementação
1. Definir estratégia no workflow: usar `on.push.branches` para disparar em `main` e em `dev`, e dentro do job de deploy usar condição ou variáveis para distinguir o ambiente (ex.: `github.ref == 'refs/heads/main'` para prod, `github.ref == 'refs/heads/dev'` para dev).
2. Configurar o nome da função Lambda por ambiente: usar variável do repositório (ex.: `vars.LAMBDA_FUNCTION_NAME` para prod e `vars.LAMBDA_FUNCTION_NAME_DEV` para dev) ou secrets distintos, e no job de deploy definir `LAMBDA_FUNCTION_NAME` com base na branch.
3. Garantir que o job **build-and-test** rode em ambos os branches (main e dev) antes do deploy; o job **deploy** só rode quando houver push em `main` (deploy prod) ou em `dev` (deploy dev), sem deploy em outras branches (a menos que seja desejado manter workflow_dispatch para deploy manual).
4. Documentar no README ou em `docs/` que: main → produção; dev → desenvolvimento; e quais variáveis (LAMBDA_FUNCTION_NAME, LAMBDA_FUNCTION_NAME_DEV) precisam estar configuradas no repositório.

## Formas de teste
- Fazer push em uma branch `dev` e verificar que o workflow dispara e que o deploy atualiza a função Lambda de **dev** (e não a de prod).
- Fazer push (ou merge) em `main` e verificar que o deploy atualiza a função Lambda de **produção**.
- Verificar que pushes em outras branches (ex.: feature/xyz) não disparam deploy, ou que o comportamento está documentado.

## Critérios de aceite da subtask
- [x] Push para **main** dispara o workflow e o deploy atualiza a função Lambda (nome configurável via variável `LAMBDA_FUNCTION_NAME`).
- [x] Push para **dev** dispara o workflow e o deploy atualiza a mesma função Lambda.
- [x] A estratégia de branches (main e dev deployam na mesma função) está documentada no repositório.
- [x] Build e testes rodam em ambos os branches antes do deploy; o deploy só executa quando a branch é `main` ou `dev`, conforme regra acima.
