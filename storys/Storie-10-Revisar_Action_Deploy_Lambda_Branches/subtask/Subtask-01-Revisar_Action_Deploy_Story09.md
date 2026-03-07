# Subtask 01: Revisar action para deploy do Lambda da Story 09

## Descrição
Garantir que o workflow `.github/workflows/deploy-lambda.yml` empacote e publique corretamente o Lambda da Story 09 (ProcessChunkUseCase, S3, Application/Infra), e que o handler e paths estejam alinhados ao projeto atual (não mais Hello World).

## Passos de implementação
1. Abrir `.github/workflows/deploy-lambda.yml` e confirmar que o job de **Package Lambda** usa o path do projeto correto: `src/InterfacesExternas/VideoProcessor.Lambda` (ou o path relativo à raiz do repositório usado hoje).
2. Verificar se `dotnet lambda package` inclui todas as dependências (Application, Domain, Infra) via project references; em caso de falha de build ou pacote incompleto, ajustar comando ou propriedades do csproj (ex.: `CopyLocalLockFileAssemblies` já presente).
3. Confirmar que o passo **Set Lambda handler** usa o handler correto: `VideoProcessor.Lambda::VideoProcessor.Lambda.Function::FunctionHandler` (ou o valor atual usado pelo `Function.cs`).
4. Revisar variáveis de ambiente do job (ex.: `LAMBDA_FUNCTION_NAME`) e garantir que estejam documentadas ou que o default seja coerente com o nome da função na AWS.
5. Executar o workflow em modo de teste (branch ou workflow_dispatch) e corrigir erros de build/package/update até o deploy concluir com sucesso.

## Formas de teste
- Rodar o workflow no GitHub Actions (push em branch de teste ou workflow_dispatch) e verificar que o job **deploy** termina em sucesso.
- Após o deploy, invocar a função na AWS com um payload `ChunkProcessorInput` válido e verificar que a resposta é um `ChunkProcessorOutput` (ou que a função não falha por assembly/configuração).
- Conferir no console AWS que a função foi atualizada (Last modified) e que o handler está correto.

## Critérios de aceite da subtask
- [x] O comando `dotnet lambda package` no workflow gera o ZIP sem erros e o projeto referenciado é `VideoProcessor.Lambda` com refs Core/Infra.
- [x] O handler configurado na Lambda via workflow é `VideoProcessor.Lambda::VideoProcessor.Lambda.Function::FunctionHandler`.
- [x] O job de deploy (update-function-code e update-function-configuration) conclui com sucesso no GitHub Actions.
- [x] Nenhum passo da action assume comportamento ou paths do antigo Lambda “Hello World” de forma incompatível com a Story 09.
