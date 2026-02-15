# Storie-03: Contrato de Input/Output + Versionamento (contractVersion)

## Status
- **Estado:** ✅ Concluída
- **Data de Conclusão:** 15/02/2026

## Descrição
Como desenvolvedor do Lambda Worker, quero definir contratos tipados para input e output do handler usando records C# 13 e implementar versionamento via `contractVersion`, para garantir evolução segura do contrato e compatibilidade com Step Functions ao longo do tempo.

## Objetivo
Criar models (records) para `ChunkProcessorInput` e `ChunkProcessorOutput`, implementar deserialização/serialização com System.Text.Json, validar `contractVersion` no handler, e retornar output estruturado (SUCCEEDED/FAILED) com todas as informações necessárias para o fan-in da Step Functions.

## Escopo Técnico
- **Tecnologias:** C# 13 records, System.Text.Json, .NET 10
- **Arquivos criados/modificados:**
  - `src/VideoProcessor.Domain/Models/ChunkProcessorInput.cs`
  - `src/VideoProcessor.Domain/Models/ChunkProcessorOutput.cs`
  - `src/VideoProcessor.Domain/Models/ChunkInfo.cs`
  - `src/VideoProcessor.Domain/Models/SourceInfo.cs`
  - `src/VideoProcessor.Domain/Models/OutputConfig.cs`
  - `src/VideoProcessor.Domain/Models/ManifestInfo.cs`
  - `src/VideoProcessor.Domain/Models/ErrorInfo.cs`
  - `src/VideoProcessor.Domain/Exceptions/UnsupportedContractVersionException.cs`
  - `src/VideoProcessor.Lambda/Function.cs` (atualizar para usar models)
  - `tests/VideoProcessor.Tests.Unit/Domain/Models/` (testes de serialização)
- **Componentes:** Models de contrato, exceções de versão, lógica de versionamento
- **Pacotes/Dependências:**
  - System.Text.Json (já incluído no .NET 10)
  - FluentValidation (11.9.0) — para próxima story, preparar aqui

## Dependências e Riscos (para estimativa)
- **Dependências:**
  - Storie-01 concluída (estrutura de projetos)
- **Riscos:**
  - Contrato pode mudar conforme necessidades da Step Functions (mitigar: versionamento bem definido)
  - System.Text.Json pode ter limitações com tipos complexos (mitigar: testar deserialização desde o início)
- **Pré-condições:**
  - Definição clara dos campos obrigatórios vs opcionais no contrato

## Subtasks
- [x] [Subtask 01: Criar models de input (ChunkProcessorInput e dependentes)](./subtask/Subtask-01-Criar_Models_Input.md)
- [x] [Subtask 02: Criar models de output (ChunkProcessorOutput e dependentes)](./subtask/Subtask-02-Criar_Models_Output.md)
- [x] [Subtask 03: Implementar versionamento e exceções de versão não suportada](./subtask/Subtask-03-Implementar_Versionamento.md)
- [x] [Subtask 04: Atualizar Function handler para usar models tipados](./subtask/Subtask-04-Atualizar_Handler_Models_Tipados.md)
- [x] [Subtask 05: Criar testes unitários de serialização/deserialização](./subtask/Subtask-05-Testes_Serializacao.md)

## Critérios de Aceite da História
- [x] Records criados para input: `ChunkProcessorInput`, `ChunkInfo`, `SourceInfo`, `OutputConfig`
- [x] Records criados para output: `ChunkProcessorOutput`, `ManifestInfo`, `ErrorInfo`
- [x] Campo `contractVersion` validado no handler; versões suportadas: "1.0"
- [x] Handler deserializa `JsonDocument` para `ChunkProcessorInput` e serializa `ChunkProcessorOutput` para `JsonDocument`
- [x] Output contém todos os campos: `chunkId`, `status` (SUCCEEDED/FAILED), `framesCount`, `manifest`, `error` (quando aplicável)
- [x] Exceção `UnsupportedContractVersionException` lançada para versões desconhecidas
- [x] Testes unitários cobrem: (a) deserialização de input válido, (b) serialização de output, (c) validação de contractVersion, (d) tratamento de versão inválida
- [x] Handler executa localmente com payload de teste retornando output mockado estruturado

## Rastreamento (dev tracking)
- **Início:** 15/02/2026, às 16:37 (Brasília)
- **Fim:** 15/02/2026, às 16:47 (Brasília)
- **Tempo total de desenvolvimento:** 10min
