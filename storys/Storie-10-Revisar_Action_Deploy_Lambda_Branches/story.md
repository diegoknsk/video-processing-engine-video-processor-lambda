# Storie-10: Revisar Action de Deploy — Lambda Story 09 e Estratégia de Branches (Main x Dev)

## Status
- **Estado:** ✅ Concluída
- **Data de Conclusão:** 07/03/2026

## Descrição
Como DevOps do Video Processing Engine, quero que a GitHub Action de deploy seja revisada para que o Lambda da Story 09 (processamento real com S3, ProcessChunkUseCase, FFmpeg) seja implantado corretamente, e que o deploy automático para produção ocorra somente ao subir código na branch **main**, enquanto pushes na branch **dev** atualizem automaticamente o ambiente de desenvolvimento.

## Objetivo
Revisar e ajustar o workflow `.github/workflows/deploy-lambda.yml` que antes implantava um Lambda “Hello World”, para que: (1) o pacote e a configuração da Lambda da Story 09 (com dependências Core/Infra, handler correto, e suporte a FFmpeg quando aplicável) subam sem falhas; (2) a estratégia de branches fique explícita: **main** → deploy produção, **dev** → deploy automático para ambiente de desenvolvimento.

## Contexto
- **Antes:** a action foi criada para um Lambda simples (Hello World).
- **Story 09:** entregou Lambda com `ProcessChunkUseCase`, `IS3VideoStorage`, S3, Xabe.FFmpeg, referências a `VideoProcessor.Application`, `VideoProcessor.Domain`, `VideoProcessor.Infra`.
- **Problema:** a action pode não estar alinhada ao projeto atual (paths, handler, layers, env) e não distingue deploy produção vs dev por branch.

## Escopo Técnico
- **Tecnologias:** GitHub Actions, AWS Lambda, .NET 10, Amazon.Lambda.Tools
- **Arquivos afetados:**
  - `.github/workflows/deploy-lambda.yml` (revisão e possíveis novos workflows ou jobs condicionais por branch)
  - Documentação em `docs/` ou README com estratégia de branches e variáveis (LAMBDA_FUNCTION_NAME por ambiente)
- **Componentes:** workflow(s) de deploy, jobs condicionais por branch (main vs dev), variáveis por ambiente
- **Pacotes/Dependências:** já existentes no projeto (dotnet 10, Lambda Tools); nenhum pacote novo obrigatório

## Dependências e Riscos (para estimativa)
- **Dependências:** Storie-09 concluída (Lambda com processamento real já implementado).
- **Riscos:** Nome da função Lambda diferente entre dev e prod (usar vars por branch ou secrets); FFmpeg no Lambda pode exigir Layer ou variável `FFMPEG_PATH` configurada na função — a action não precisa empacotar o binário, mas a doc deve deixar claro o que a função espera.
- **Pré-condições:** Secrets AWS configurados no repositório; função(s) Lambda existente(s) na AWS (uma para dev, uma para prod, ou mesmo nome com alias).

## Subtasks
- [x] [Subtask 01: Revisar action para deploy do Lambda da Story 09](./subtask/Subtask-01-Revisar_Action_Deploy_Story09.md)
- [x] [Subtask 02: Deploy produção somente em main; deploy dev em branch dev](./subtask/Subtask-02-Branches_Main_Dev_Deploy.md)
- [x] [Subtask 03: Documentar Layer FFmpeg e variáveis da Lambda](./subtask/Subtask-03-Layer_FFmpeg_Variaveis_Lambda.md)
- [x] [Subtask 04: Validar pipeline e checklist pós-deploy](./subtask/Subtask-04-Validar_Pipeline_Checklist.md)

## Critérios de Aceite da História

### Funcionais
- [x] O workflow de deploy gera o pacote ZIP do projeto `VideoProcessor.Lambda` (incluindo referências Core/Infra) e atualiza a função Lambda na AWS sem erro.
- [x] O handler da Lambda permanece ou é configurado corretamente: `VideoProcessor.Lambda::VideoProcessor.Lambda.Function::FunctionHandler`.
- [x] Deploy para **produção** (função Lambda de prod) ocorre **somente** quando há push (ou merge) para a branch **main**.
- [x] Deploy para **desenvolvimento** ocorre automaticamente quando há push para a branch **dev** (ambiente dev atualizado automaticamente).
- [x] Build e testes (job existente) continuam passando antes do deploy em ambos os fluxos.

### Técnicos / Configuração
- [x] A action usa o path correto do projeto Lambda: `src/InterfacesExternas/VideoProcessor.Lambda`.
- [x] Nome da função Lambda configurável via variável do repositório `LAMBDA_FUNCTION_NAME` (main e dev deployam na mesma função).
- [x] Documentação descreve a estratégia de branches (main e dev) e quais variáveis/secrets são necessárias.
- [x] Se a Lambda em AWS usar Layer para FFmpeg, a documentação descreve como anexar o layer à função (ou que a action não altera layers; apenas código).

### Validação
- [x] Execução manual ou automática do workflow em branch `dev` atualiza a função configurada em `LAMBDA_FUNCTION_NAME`.
- [x] Execução do workflow em branch `main` atualiza a mesma função; após o deploy, invocação com payload `ChunkProcessorInput` retorna `ChunkProcessorOutput` coerente (ou documentar que a validação E2E fica para outro momento).

## Rastreamento (dev tracking)
- **Início:** 07/03/2026, às 17:34 (Brasília)
- **Fim:** 07/03/2026, às 17:43 (Brasília)
- **Tempo total de desenvolvimento:** 9min
