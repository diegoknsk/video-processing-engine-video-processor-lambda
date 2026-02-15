# Subtask 04: Criar ADRs e Documentação de Convenções

## Descrição
Criar Architecture Decision Records (ADRs) documentando principais decisões técnicas (handler puro, idempotência, classificação de erros), e expandir documentação de convenções S3.

## Passos de Implementação
1. Criar `docs/architecture/ADR-001-handler-puro-sem-api-hosting.md`:
   ```markdown
   # ADR-001: Handler Lambda Puro sem AddAWSLambdaHosting
   
   ## Status
   Aceito
   
   ## Contexto
   O Lambda Video Processor é um worker minimalista que processa APENAS 1 chunk por invocação, executado no Map da Step Functions. Não é API HTTP, não precisa de roteamento, middleware ou framework web.
   
   ## Decisão
   Implementar handler puro (`FunctionHandler(JsonDocument, ILambdaContext)`) sem usar `AddAWSLambdaHosting` ou ASP.NET Core hosting.
   
   ## Consequências
   - **Positivo:** Simplifica código, reduz dependencies, menor cold start, mais explícito
   - **Positivo:** Não precisa de API Gateway, GATEWAY_STAGE, ou configurações de roteamento
   - **Negativo:** Sem middleware framework (implementar try-catch manual, DI manual)
   - **Mitigação:** DI configurado no construtor; tratamento de exceções centralizado no handler
   
   ## Alternativas Consideradas
   - **Alternativa A (escolhida):** Handler puro minimalista
   - **Alternativa B:** AddAWSLambdaHosting + Controllers → descartada por overhead desnecessário
   ```
2. Criar `docs/architecture/ADR-002-idempotencia-done-marker.md`:
   ```markdown
   # ADR-002: Idempotência com Done Marker no S3
   
   ## Status
   Aceito
   
   ## Contexto
   Step Functions pode reinvocar Lambda em caso de retry. Sem idempotência, chunk seria reprocessado, gerando artefatos duplicados e desperdício de recursos.
   
   ## Decisão
   Implementar idempotência usando marker `done.json` no S3. Antes de processar chunk, verificar se `done.json` existe; se sim, retornar SUCCEEDED sem reprocessar.
   
   ## Consequências
   - **Positivo:** Reprocessamentos não duplicam trabalho
   - **Positivo:** Simples de implementar e verificar (S3 HEAD operation)
   - **Negativo:** Race condition teórica (2 Lambdas processando mesmo chunk simultaneamente)
   - **Mitigação:** Step Functions Map não deve invocar mesmo chunk 2x; documentar comportamento
   
   ## Convenção de Prefixos
   - Prefixo: `{manifestPrefix}/{chunkId}/`
   - Chaves: `done.json`, `manifest.json`
   - Exemplo: `manifests/video-123/chunk-0/done.json`
   ```
3. Criar `docs/architecture/ADR-003-classificacao-erros-retryable.md`:
   ```markdown
   # ADR-003: Classificação de Erros como Retryable vs Não-Retryable
   
   ## Status
   Aceito
   
   ## Contexto
   Step Functions aplica retry automático para exceções. Precisamos diferenciar erros permanentes (input inválido) de erros transitórios (S3 503) para evitar retries desnecessários.
   
   ## Decisão
   Classificar exceções:
   - **Não-retryable:** ChunkValidationException, UnsupportedContractVersionException, AmazonS3Exception (404, 403)
   - **Retryable:** AmazonS3Exception (500, 503, throttling), HttpRequestException
   - **Estratégia:** Erros não-retryable retornam FAILED com error; erros retryable re-lançam exceção
   
   ## Consequências
   - **Positivo:** Reduz retries inúteis (validação nunca vai passar no retry)
   - **Positivo:** Permite Step Functions tratar erros transitórios automaticamente
   - **Negativo:** Classificação incorreta pode causar falha permanente ou loop infinito
   - **Mitigação:** Testes exaustivos de classificação; conservador (unknown → não-retryable)
   ```
4. Expandir `docs/S3_CONVENTIONS.md` (criado na Storie-04) com exemplos completos e edge cases

## Formas de Teste
1. **Revisão de documentação:** verificar que ADRs seguem formato padrão (Status, Contexto, Decisão, Consequências, Alternativas)
2. **Validação técnica:** confirmar que decisões documentadas correspondem ao código implementado
3. **Legibilidade:** pedir feedback de outro desenvolvedor

## Critérios de Aceite da Subtask
- [ ] ADR-001 criado documentando decisão de handler puro
- [ ] ADR-002 criado documentando idempotência com done marker
- [ ] ADR-003 criado documentando classificação de erros retryable
- [ ] Cada ADR segue formato: Status, Contexto, Decisão, Consequências, Alternativas
- [ ] `docs/S3_CONVENTIONS.md` expandido com exemplos e edge cases
- [ ] Documentos linkados no README (seção Arquitetura)
