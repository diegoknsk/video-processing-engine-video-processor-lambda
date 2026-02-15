# Subtask 02: Configurar Logging Estruturado e Enriquecer Logs com Correlation

## Descrição
Configurar `ILogger` para logs estruturados (formato JSON no CloudWatch), criar extension method para enriquecer logs com correlation context automaticamente, e aplicar em todos os pontos de log.

## Passos de Implementação
1. Criar `src/VideoProcessor.Application/Extensions/LoggerExtensions.cs`:
   ```csharp
   public static class LoggerExtensions
   {
       public static void LogInformationWithContext(this ILogger logger, string message)
       {
           var context = CorrelationContext.Current;
           if (context != null)
           {
               using (logger.BeginScope(context.ToDictionary()))
               {
                   logger.LogInformation(message);
               }
           }
           else
           {
               logger.LogInformation(message);
           }
       }
       
       public static void LogErrorWithContext(this ILogger logger, Exception ex, string message)
       {
           var context = CorrelationContext.Current;
           if (context != null)
           {
               using (logger.BeginScope(context.ToDictionary()))
               {
                   logger.LogError(ex, message);
               }
           }
           else
           {
               logger.LogError(ex, message);
           }
       }
       
       public static void LogWarningWithContext(this ILogger logger, string message)
       {
           var context = CorrelationContext.Current;
           if (context != null)
           {
               using (logger.BeginScope(context.ToDictionary()))
               {
                   logger.LogWarning(message);
               }
           }
           else
           {
               logger.LogWarning(message);
           }
       }
   }
   ```
2. No DI, configurar logging para formato JSON (se necessário):
   ```csharp
   services.AddLogging(builder =>
   {
       builder.AddJsonConsole(options =>
       {
           options.IncludeScopes = true;
           options.JsonWriterOptions = new JsonWriterOptions { Indented = false };
       });
   });
   ```
   (Nota: Lambda já usa JSON por padrão via CloudWatch; essa configuração pode ser opcional)
3. Refatorar logs existentes para usar `LogInformationWithContext`, `LogErrorWithContext`, etc.

## Formas de Teste
1. **Teste local:** executar handler localmente, verificar que logs incluem videoId, chunkId, correlationId
2. **Mock logger:** capturar logs em teste unitário, verificar que scope contém correlation context
3. **CloudWatch:** após deploy, verificar que logs aparecem estruturados com campos customizados

## Critérios de Aceite da Subtask
- [ ] Extension methods criados: `LogInformationWithContext`, `LogErrorWithContext`, `LogWarningWithContext`
- [ ] Logs usam `BeginScope` com `CorrelationContext.ToDictionary()` para incluir contexto
- [ ] Logging configurado para incluir scopes (IncludeScopes = true)
- [ ] Todos os logs no handler e use case refatorados para usar extension methods
- [ ] Testes unitários verificam que scope é criado com correlation context
- [ ] Logs locais incluem campos: videoId, chunkId, executionArn, correlationId
