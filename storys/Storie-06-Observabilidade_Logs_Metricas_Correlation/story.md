# Storie-06: Observabilidade (Logs Estruturados + Métricas + Correlation)

## Status
- **Estado:** 🔄 Em desenvolvimento
- **Data de Conclusão:** [DD/MM/AAAA]

## Descrição
Como desenvolvedor e operador do sistema, quero implementar logging estruturado com correlation (videoId, chunkId, executionArn), métricas customizadas (duração, sucesso/falha), e enriquecimento de logs com contexto, para facilitar troubleshooting, monitoramento e análise de performance do Lambda Worker.

## Objetivo
Configurar Microsoft.Extensions.Logging para logs estruturados JSON no CloudWatch, criar middleware de correlation que propaga videoId/chunkId/executionArn em todos os logs, implementar métricas customizadas (duração de processamento, taxa de sucesso/falha), e adicionar logs em pontos-chave do fluxo (início, validação, S3 operations, fim).

## Escopo Técnico
- **Tecnologias:** Microsoft.Extensions.Logging, AWS CloudWatch Logs, métricas customizadas (EMF - Embedded Metric Format ou CloudWatch PutMetricData)
- **Arquivos criados/modificados:**
  - `src/VideoProcessor.Application/Services/CorrelationContext.cs`
  - `src/VideoProcessor.Application/Services/MetricsPublisher.cs`
  - `src/VideoProcessor.Infra/Services/CloudWatchMetricsPublisher.cs` (implementação)
  - `src/VideoProcessor.Lambda/Function.cs` (adicionar correlation e métricas)
  - `src/VideoProcessor.Application/UseCases/ProcessChunkUseCase.cs` (adicionar logs estruturados)
  - `tests/VideoProcessor.Tests.Unit/Application/Services/MetricsPublisherTests.cs`
- **Componentes:** CorrelationContext, MetricsPublisher, logs estruturados, enriquecimento de logs
- **Pacotes/Dependências:**
  - Microsoft.Extensions.Logging (já incluído no .NET 10)
  - AWS.Logger.Core (opcional, para formatação CloudWatch)
  - AWSSDK.CloudWatch (3.7.400 ou superior) — para métricas customizadas

## Dependências e Riscos (para estimativa)
- **Dependências:**
  - Storie-04 concluída (use case base)
  - Storie-05 concluída (tratamento de erros)
- **Riscos:**
  - Logs excessivos podem aumentar custo CloudWatch (mitigar: definir log level adequado)
  - Métricas customizadas têm custo (mitigar: limitar a métricas essenciais)
- **Pré-condições:**
  - Lambda com permissão para escrever logs no CloudWatch (já existe por padrão)
  - Lambda com permissão para PutMetricData no CloudWatch (se usar métricas customizadas)

## Subtasks
- [Subtask 01: Criar CorrelationContext para propagar videoId/chunkId/executionArn](./subtask/Subtask-01-Criar_CorrelationContext.md)
- [Subtask 02: Configurar logging estruturado e enriquecer logs com correlation](./subtask/Subtask-02-Configurar_Logging_Estruturado.md)
- [Subtask 03: Adicionar logs em pontos-chave do fluxo](./subtask/Subtask-03-Adicionar_Logs_Pontos_Chave.md)
- [Subtask 04: Criar MetricsPublisher e publicar métricas customizadas](./subtask/Subtask-04-Criar_MetricsPublisher.md)
- [Subtask 05: Validar logs e métricas no CloudWatch](./subtask/Subtask-05-Validar_Logs_Metricas_CloudWatch.md)

## Critérios de Aceite da História
- [ ] `CorrelationContext` criado com propriedades: VideoId, ChunkId, ExecutionArn, CorrelationId (GUID)
- [ ] Correlation context populado no início do handler e propagado via AsyncLocal ou ILogger scope
- [ ] Logs estruturados incluem campos: videoId, chunkId, executionArn, correlationId, timestamp, level, message
- [ ] Logs adicionados em pontos-chave: (a) início do handler, (b) após validação, (c) verificação de idempotência, (d) escrita de artefatos S3, (e) fim (sucesso/falha)
- [ ] `MetricsPublisher` publica métricas: (a) duração de processamento (ms), (b) sucesso/falha (count), (c) chunks processados (count)
- [ ] Métricas publicadas no CloudWatch com namespace customizado (ex.: `VideoProcessing/ChunkWorker`)
- [ ] Testes unitários cobrem: correlation context, logs estruturados, publicação de métricas
- [ ] Logs visíveis no CloudWatch Logs Insights com queries filtrando por videoId/chunkId
- [ ] Métricas visíveis no CloudWatch Metrics com namespace customizado

## Rastreamento (dev tracking)
- **Início:** —
- **Fim:** —
- **Tempo total de desenvolvimento:** —
