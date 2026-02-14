# Subtask 02: Criar Exceção ChunkValidationException (Não-Retryable)

## Descrição
Criar exceção customizada `ChunkValidationException` que encapsula erros de validação do FluentValidation, marcada como não-retryable, e contém mensagens de erro estruturadas.

## Passos de Implementação
1. Criar `src/VideoProcessor.Domain/Exceptions/ChunkValidationException.cs`:
   ```csharp
   using FluentValidation.Results;
   
   public class ChunkValidationException : Exception
   {
       public IReadOnlyList<string> ValidationErrors { get; }
       
       public ChunkValidationException(IEnumerable<ValidationFailure> failures)
           : base("Input validation failed")
       {
           ValidationErrors = failures.Select(f => f.ErrorMessage).ToList();
       }
       
       public ChunkValidationException(string message)
           : base(message)
       {
           ValidationErrors = new[] { message };
       }
       
       public override string Message => 
           $"{base.Message}: {string.Join("; ", ValidationErrors)}";
   }
   ```
2. No `ProcessChunkUseCase`, adicionar validação no início:
   ```csharp
   public async Task<ChunkProcessorOutput> ExecuteAsync(ChunkProcessorInput input, CancellationToken ct)
   {
       // Validar input
       var validationResult = await _validator.ValidateAsync(input, ct);
       if (!validationResult.IsValid)
       {
           throw new ChunkValidationException(validationResult.Errors);
       }
       
       // ... resto do código
   }
   ```
3. Injetar `IValidator<ChunkProcessorInput>` no construtor do use case:
   ```csharp
   public class ProcessChunkUseCase(
       IS3Service s3Service,
       IPrefixBuilder prefixBuilder,
       IValidator<ChunkProcessorInput> validator,
       ILogger<ProcessChunkUseCase> logger
   ) : IProcessChunkUseCase
   ```

## Formas de Teste
1. **Teste unitário:** criar input inválido, chamar use case, verificar que `ChunkValidationException` é lançada
2. **Teste de mensagem:** verificar que exceção contém lista de erros de validação
3. **Mock validator:** em outros testes, mockar validator para simular falha

## Critérios de Aceite da Subtask
- [ ] `ChunkValidationException` criada com propriedade `ValidationErrors`
- [ ] Exceção encapsula `ValidationFailure` do FluentValidation
- [ ] Mensagem de erro concatena todos os erros de validação
- [ ] Use case valida input e lança `ChunkValidationException` se inválido
- [ ] Validator injetado no construtor do use case
- [ ] Teste unitário confirma que exceção é lançada para input inválido
