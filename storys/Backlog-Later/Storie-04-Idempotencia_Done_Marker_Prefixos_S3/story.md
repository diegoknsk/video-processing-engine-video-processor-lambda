# Storie-04: Idempotência (done.json marker) + Convenção de Prefixos S3

> ⚠️ **STATUS: PAUSADA** – Esta story foi movida para o backlog. Executar após:
> - Processamento real de vídeo implementado (Storie-05)
> - Processamento rodando no Lambda (Storie-06)
> - Integração S3 básica funcionando (Storie-07)
> 
> Motivo: Idempotência é uma robustez avançada que deve ser implementada após o fluxo básico funcionar.

## Status
- **Estado:** ⏸️ Pausada
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do Lambda Worker, quero implementar idempotência usando marker `done.json` no S3 e definir convenção determinística de prefixos, para garantir que reprocessamentos (retries da Step Functions) não dupliquem trabalho e que artefatos sejam organizados de forma previsível.

## Objetivo
Implementar verificação de `done.json` antes de processar chunk (se existir, retornar SUCCEEDED sem reprocessar), criar serviço S3 para leitura/escrita de artefatos, definir convenção de prefixos `{manifestPrefix}/{chunkId}/`, escrever `manifest.json` e `done.json` ao final, e garantir que handler é idempotente.

## Escopo Técnico
- **Tecnologias:** AWSSDK.S3, C# 13, async I/O
- **Arquivos criados/modificados:**
  - `src/VideoProcessor.Domain/Ports/IS3Service.cs` (port para S3)
  - `src/VideoProcessor.Infra/Services/S3Service.cs` (implementação)
  - `src/VideoProcessor.Application/UseCases/ProcessChunkUseCase.cs`
  - `src/VideoProcessor.Application/Services/PrefixBuilder.cs`
  - `src/VideoProcessor.Lambda/Function.cs` (chamar use case)
  - `tests/VideoProcessor.Tests.Unit/Application/Services/PrefixBuilderTests.cs`
  - `tests/VideoProcessor.Tests.Unit/Application/UseCases/ProcessChunkUseCaseTests.cs`
- **Componentes:** S3Service, PrefixBuilder, ProcessChunkUseCase, lógica de idempotência
- **Pacotes/Dependências:**
  - AWSSDK.S3 (3.7.400 ou superior) — já incluído na Storie-01

## Dependências e Riscos (para estimativa)
- **Dependências:**
  - Storie-01 concluída (estrutura base)
  - Storie-03 concluída (models de input/output)
- **Riscos:**
  - Race condition: dois lambdas processando mesmo chunk simultaneamente (mitigar: Step Functions deve evitar isso; documentar)
  - S3 eventual consistency (não mais relevante após strong consistency; sanity check)
  - Falha ao escrever done.json pode causar reprocessamento (aceitável; operação final)
- **Pré-condições:**
  - Bucket S3 criado e Lambda com permissões (GetObject, PutObject)

## Subtasks
- [Subtask 01: Criar port IS3Service e implementação S3Service](./subtask/Subtask-01-Criar_Port_S3Service.md)
- [Subtask 02: Criar PrefixBuilder para convenção de prefixos determinística](./subtask/Subtask-02-Criar_PrefixBuilder.md)
- [Subtask 03: Implementar ProcessChunkUseCase com verificação de idempotência](./subtask/Subtask-03-Implementar_UseCase_Idempotencia.md)
- [Subtask 04: Escrever manifest.json e done.json mockados no S3](./subtask/Subtask-04-Escrever_Manifest_Done_S3.md)
- [Subtask 05: Criar testes unitários de idempotência e prefixos](./subtask/Subtask-05-Testes_Idempotencia_Prefixos.md)

## Critérios de Aceite da História
- [ ] Port `IS3Service` criado com métodos: `ExistsAsync(bucket, key)`, `GetJsonAsync<T>(bucket, key)`, `PutJsonAsync<T>(bucket, key, obj)`
- [ ] Implementação `S3Service` usando AWSSDK.S3 (AmazonS3Client)
- [ ] `PrefixBuilder` gera prefixo: `{manifestPrefix}/{chunkId}/` (determinístico)
- [ ] `ProcessChunkUseCase` verifica `done.json`; se existir, retorna SUCCEEDED sem reprocessar
- [ ] Se não existir, processa (mock), escreve `manifest.json` e `done.json`, retorna SUCCEEDED
- [ ] `manifest.json` contém estrutura básica: `{ "chunkId": "...", "status": "completed", "framesCount": 0 }`
- [ ] `done.json` é marker vazio ou com timestamp: `{ "completedAt": "..." }`
- [ ] Testes unitários cobrem: (a) chunk já processado (done.json existe) não reprocessa, (b) chunk novo processa e grava artefatos, (c) PrefixBuilder gera prefixos corretos
- [ ] Handler integrado com use case executa localmente e grava artefatos no S3

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —
