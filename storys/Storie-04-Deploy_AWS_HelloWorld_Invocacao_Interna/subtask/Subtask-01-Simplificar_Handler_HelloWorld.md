# Subtask 01: Simplificar handler para retornar Hello World

## Status
- [ ] Não iniciado
- [ ] Em progresso
- [x] Concluído

## Descrição
Atualizar `Function.cs` para retornar um payload JSON simples e informativo tipo "Hello World", removendo qualquer lógica complexa de processamento. Adicionar log estruturado indicando que a função foi invocada.

## Tarefas
1. Abrir `src/VideoProcessor.Lambda/Function.cs`
2. Simplificar método `FunctionHandler` para:
   - Receber payload genérico (string ou objeto simples)
   - Logar no CloudWatch: `"Hello World invoked at {timestamp}"`
   - Retornar JSON: `{ "message": "Hello World from Video Processor Lambda", "version": "1.0.0", "timestamp": "...", "environment": "dev" }`
3. Remover referências a processamento de vídeo (se houver)
4. Garantir que o código compila: `dotnet build`

## Critérios de Aceite
- [ ] `FunctionHandler` retorna payload Hello World estruturado
- [ ] Log inclui timestamp e mensagem "Hello World invoked"
- [ ] Código compila sem erros
- [ ] Assinatura do handler permanece compatível com Lambda: `Task<string> FunctionHandler(string input, ILambdaContext context)`

## Notas Técnicas
- Usar `ILambdaContext.Logger.LogInformation()` para logs
- Serializar resposta com `System.Text.Json.JsonSerializer.Serialize()`
- Timestamp pode usar `DateTime.UtcNow.ToString("o")` (formato ISO 8601)
