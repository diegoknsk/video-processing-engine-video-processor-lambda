# Storie-05: Validação de Input + Classificação de Erros (retryable vs não-retryable)

> ⚠️ **STATUS: PAUSADA** – Esta story foi movida para o backlog. Executar após:
> - Processamento real de vídeo implementado (Storie-05 nova)
> - Processamento rodando no Lambda (Storie-06 nova)
> - Integração S3 básica funcionando (Storie-07 nova)
> 
> Motivo: Classificação formal de erros é uma robustez avançada que deve ser implementada após o fluxo básico funcionar.

## Status
- **Estado:** ⏸️ Pausada
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor do Lambda Worker, quero validar estruturalmente o input usando FluentValidation e classificar erros como retryable ou não-retryable, para garantir que inputs inválidos sejam rejeitados imediatamente (não-retryable) e erros transitórios de infraestrutura permitam retry da Step Functions.

## Objetivo
Criar validators FluentValidation para `ChunkProcessorInput` validando campos obrigatórios, formatos e ranges, implementar classificação de exceções (ValidationError → não-retryable, S3/rede → retryable), tratar exceções no handler retornando FAILED com `error.retryable` correto, e lançar exceções retryables para Step Functions aplicar retry.

## Escopo Técnico
- **Tecnologias:** FluentValidation, exception handling, C# 13
- **Arquivos criados/modificados:**
  - `src/VideoProcessor.Application/Validators/ChunkProcessorInputValidator.cs`
  - `src/VideoProcessor.Domain/Exceptions/ChunkValidationException.cs`
  - `src/VideoProcessor.Application/Services/ErrorClassifier.cs`
  - `src/VideoProcessor.Lambda/Function.cs` (adicionar try-catch global)
  - `tests/VideoProcessor.Tests.Unit/Application/Validators/ChunkProcessorInputValidatorTests.cs`
  - `tests/VideoProcessor.Tests.Unit/Application/Services/ErrorClassifierTests.cs`
- **Componentes:** Validators, ErrorClassifier, exception handling
- **Pacotes/Dependências:**
  - FluentValidation (11.9.0)

## Dependências e Riscos (para estimativa)
- **Dependências:**
  - Storie-03 concluída (models de input/output)
  - Storie-04 concluída (use case base)
- **Riscos:**
  - Classificação incorreta de erro pode causar retries desnecessários ou falha de processamento válido (mitigar: testes exaustivos)
- **Pré-condições:**
  - Definição clara de quais erros são retryable

## Subtasks
- [Subtask 01: Instalar FluentValidation e criar ChunkProcessorInputValidator](./subtask/Subtask-01-Criar_Validator_Input.md)
- [Subtask 02: Criar exceção ChunkValidationException (não-retryable)](./subtask/Subtask-02-Criar_Exception_Validation.md)
- [Subtask 03: Criar ErrorClassifier para classificar exceções](./subtask/Subtask-03-Criar_ErrorClassifier.md)
- [Subtask 04: Adicionar tratamento de exceções no handler](./subtask/Subtask-04-Tratar_Excecoes_Handler.md)
- [Subtask 05: Criar testes unitários de validação e classificação](./subtask/Subtask-05-Testes_Validacao_Classificacao.md)

## Critérios de Aceite da História
- [ ] FluentValidation instalado (versão 11.9.0)
- [ ] `ChunkProcessorInputValidator` criado validando: contractVersion não vazio, videoId não vazio, chunkId não vazio, startSec ≥ 0, endSec > startSec, bucket/key não vazios
- [ ] Exceção `ChunkValidationException` criada (não-retryable)
- [ ] `ErrorClassifier` classifica exceções: `ChunkValidationException` → não-retryable, `AmazonS3Exception` → retryable, `HttpRequestException` → retryable, outras → não-retryable
- [ ] Handler valida input antes de chamar use case; se inválido, retorna FAILED com `error.retryable = false`
- [ ] Erros retryable (S3, rede) lançam exceção não capturada → Step Functions aplica retry
- [ ] Erros não-retryable retornam FAILED com error detalhado
- [ ] Testes unitários cobrem: (a) input válido passa validação, (b) input inválido falha validação, (c) classificação de exceções conhecidas
- [ ] Handler executa localmente com input inválido e retorna FAILED com error correto

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —
