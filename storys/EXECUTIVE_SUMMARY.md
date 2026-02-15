# Resumo Executivo — Stories do Lambda Video Processor

## Estatísticas Gerais

| Métrica | Valor |
|---------|-------|
| **Total de Stories** | 7 |
| **Total de Subtasks** | 30 |
| **Média de Subtasks por Story** | 4.3 |
| **Cobertura de Código Alvo** | ≥ 80% |
| **Tecnologia Principal** | .NET 10 / C# 13 |
| **Infraestrutura** | AWS Lambda + S3 + CloudWatch |

---

## Breakdown por Story

### Storie-01: Bootstrap do Projeto
- **Subtasks:** 4
- **Foco:** Estrutura inicial, handler puro, DI, empacotamento
- **Entregáveis:** Solution completa, projetos de teste, README base
- **Complexidade:** ⭐⭐ (Média)

### Storie-02: Deploy e CI/CD
- **Subtasks:** 4
- **Foco:** GitHub Actions, validação E2E, AWS Academy
- **Entregáveis:** Pipeline completo, scripts de validação, evidências
- **Complexidade:** ⭐⭐⭐ (Alta — integração real)

### Storie-03: Contratos e Versionamento
- **Subtasks:** 5
- **Foco:** Models tipados, contractVersion, serialização
- **Entregáveis:** Records C# 13, validator de versão, testes
- **Complexidade:** ⭐⭐ (Média)

### Storie-04: Idempotência e S3
- **Subtasks:** 5
- **Foco:** Done marker, prefixos determinísticos, manifestos
- **Entregáveis:** S3Service, PrefixBuilder, use case idempotente
- **Complexidade:** ⭐⭐⭐ (Alta — lógica crítica)

### Storie-05: Validação e Erros
- **Subtasks:** 5
- **Foco:** FluentValidation, classificação retryable/não-retryable
- **Entregáveis:** Validators, ErrorClassifier, tratamento no handler
- **Complexidade:** ⭐⭐⭐ (Alta — múltiplos cenários)

### Storie-06: Observabilidade
- **Subtasks:** 5
- **Foco:** Logs estruturados, métricas, correlation
- **Entregáveis:** CorrelationContext, MetricsPublisher, logs no CloudWatch
- **Complexidade:** ⭐⭐⭐ (Alta — instrumentação completa)

### Storie-07: Testes e Documentação
- **Subtasks:** 6
- **Foco:** BDD, cobertura ≥ 80%, ADRs, troubleshooting
- **Entregáveis:** Feature SpecFlow, relatórios, ADRs, runbook
- **Complexidade:** ⭐⭐⭐⭐ (Muito Alta — múltiplas frentes)

---

## Cobertura de Requisitos

### Requisitos Funcionais (MVP)
| Requisito | Story |
|-----------|-------|
| Handler puro (sem API hosting) | Storie-01 |
| Input tipado (contractVersion, videoId, chunk, source, output) | Storie-03 |
| Output tipado (chunkId, status, framesCount, manifest, error) | Storie-03 |
| Versionamento de contrato | Storie-03 |
| Idempotência (done.json) | Storie-04 |
| Manifestos mockados no S3 | Storie-04 |
| Validação de input | Storie-05 |
| Classificação de erros | Storie-05 |

### Requisitos de Observabilidade
| Requisito | Story |
|-----------|-------|
| Logs estruturados | Storie-06 |
| Correlation (videoId, chunkId, executionArn) | Storie-06 |
| Métricas customizadas | Storie-06 |
| CloudWatch Logs | Storie-06 |
| CloudWatch Metrics | Storie-06 |

### Requisitos de DevOps
| Requisito | Story |
|-----------|-------|
| GitHub Actions pipeline | Storie-02 |
| Deploy automático | Storie-02 |
| Validação E2E pós-deploy | Storie-02 |
| AWS Academy credenciais temporárias | Storie-02 |
| Relatórios de cobertura | Storie-07 |

### Requisitos de Qualidade
| Requisito | Story |
|-----------|-------|
| ≥ 1 teste BDD (hackathon) | Storie-07 |
| Cobertura ≥ 80% | Storie-07 |
| ADRs (decisões arquiteturais) | Storie-07 |
| Troubleshooting guide | Storie-07 |
| Runbook operacional | Storie-07 |
| README completo | Storie-01, 07 |

---

## Pacotes e Dependências

### Principais Pacotes (.NET)
| Pacote | Versão | Story | Uso |
|--------|--------|-------|-----|
| Amazon.Lambda.Core | 2.3.0 | Storie-01 | Runtime Lambda |
| Amazon.Lambda.Serialization.SystemTextJson | 2.4.3 | Storie-01 | Serialização JSON |
| AWSSDK.S3 | ≥3.7.400 | Storie-01 | Cliente S3 |
| Microsoft.Extensions.DependencyInjection | 10.0.0 | Storie-01 | DI Container |
| FluentValidation | 11.9.0 | Storie-05 | Validação |
| xUnit | 2.9.0 | Storie-01 | Testes unitários |
| SpecFlow.xUnit | 3.9.74 | Storie-01 | Testes BDD |
| Moq | 4.20.0 | Storie-01 | Mocks |
| FluentAssertions | 7.0.0 | Storie-01 | Assertions |
| coverlet.collector | 6.0.0 | Storie-01 | Cobertura |
| ReportGenerator | ≥5.2.0 | Storie-07 | Relatórios HTML |

---

## Convenções e Padrões

### Clean Architecture
- **Domain:** Models, Ports, Exceptions (sem dependências externas)
- **Application:** Use Cases, Validators, Services (depende de Domain)
- **Infra:** Implementações de Ports (depende de Domain/Application)
- **Lambda:** Handler e bootstrap de DI (depende de Application/Infra)

### Nomenclatura
- **Classes:** PascalCase (ex.: `ProcessChunkUseCase`)
- **Métodos:** PascalCase (ex.: `ExecuteAsync`)
- **Variáveis locais:** camelCase (ex.: `input`, `chunkId`)
- **Campos privados:** camelCase com `_` (ex.: `_s3Service`)
- **Constantes:** UPPERCASE (ex.: `NAMESPACE`)
- **Records:** PascalCase (ex.: `ChunkProcessorInput`)

### Convenções S3
- **Estrutura de prefixos:** `{manifestPrefix}/{chunkId}/`
- **Manifest:** `{prefix}/manifest.json`
- **Done marker:** `{prefix}/done.json`
- **Exemplo:** `manifests/video-123/chunk-0/manifest.json`

---

## Cronograma Estimado

| Story | Complexidade | Estimativa | Acumulado |
|-------|--------------|------------|-----------|
| Storie-01 | ⭐⭐ | 4-6 horas | 4-6h |
| Storie-02 | ⭐⭐⭐ | 6-8 horas | 10-14h |
| Storie-03 | ⭐⭐ | 4-6 horas | 14-20h |
| Storie-04 | ⭐⭐⭐ | 6-8 horas | 20-28h |
| Storie-05 | ⭐⭐⭐ | 6-8 horas | 26-36h |
| Storie-06 | ⭐⭐⭐ | 6-8 horas | 32-44h |
| Storie-07 | ⭐⭐⭐⭐ | 8-10 horas | 40-54h |

**Total Estimado:** 40-54 horas (~1-1.5 semanas de trabalho focado)

---

## Riscos e Mitigações

| Risco | Impacto | Probabilidade | Mitigação | Story |
|-------|---------|---------------|-----------|-------|
| Credenciais AWS Academy expiram | Alto | Alta | Documentar renovação; automação se possível | Storie-02 |
| .NET 10 em preview (instável) | Médio | Média | Usar .NET 8 LTS se necessário | Storie-01 |
| Cobertura < 80% | Médio | Média | Identificar gaps cedo; refatorar para testabilidade | Storie-07 |
| Race condition (2 Lambdas mesmo chunk) | Baixo | Baixa | Step Functions evita; documentar comportamento | Storie-04 |
| Classificação incorreta de erros | Alto | Baixa | Testes exaustivos de todos os tipos de exceção | Storie-05 |
| Timeout de Lambda (processamento lento) | Médio | Baixa | Configurar timeout 30s+; monitorar duração | Storie-02 |

---

## Critérios de Sucesso

### MVP (Mínimo Viável)
- ✅ Lambda processa chunk mockado e grava manifest/done no S3
- ✅ Idempotência funciona (reprocessamento não duplica)
- ✅ Validação rejeita inputs inválidos
- ✅ Deploy via GitHub Actions funcional
- ✅ ≥ 1 teste BDD (requisito hackathon)

### Qualidade
- ✅ Cobertura de testes ≥ 80%
- ✅ Todos os testes (Unit + BDD) passam
- ✅ Pipeline CI/CD verde
- ✅ Logs estruturados visíveis no CloudWatch

### Documentação
- ✅ README completo com todas as seções
- ✅ ≥ 3 ADRs documentando decisões-chave
- ✅ Troubleshooting guide com ≥ 5 problemas
- ✅ Runbook operacional completo

### Observabilidade
- ✅ Métricas customizadas no CloudWatch
- ✅ Queries Logs Insights funcionam
- ✅ Correlation em todos os logs

---

## Próximas Fases (Pós-MVP)

1. **Extração Real de Frames:** Integrar FFmpeg para extração de frames (skill já existe)
2. **Upload de Frames:** Escrever frames extraídos no S3
3. **Otimizações:** Memory pooling, Span<T>, compiled queries
4. **Retry Policies:** Exponential backoff, circuit breaker
5. **Integração Completa:** Conectar com outros componentes do pipeline

---

**Documento gerado em:** 14/02/2026  
**Versão:** 1.0  
**Status:** Planejamento concluído — pronto para desenvolvimento
