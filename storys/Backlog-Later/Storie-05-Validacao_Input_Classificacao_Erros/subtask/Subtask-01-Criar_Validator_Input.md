# Subtask 01: Instalar FluentValidation e Criar ChunkProcessorInputValidator

## Descrição
Instalar FluentValidation no projeto Application, criar validator para `ChunkProcessorInput` validando todos os campos obrigatórios e regras de negócio (ranges, formatos), e registrar no DI.

## Passos de Implementação
1. Instalar FluentValidation no projeto Application:
   ```bash
   cd src/VideoProcessor.Application
   dotnet add package FluentValidation --version 11.9.0
   ```
2. Criar `src/VideoProcessor.Application/Validators/ChunkProcessorInputValidator.cs`:
   ```csharp
   using FluentValidation;
   
   public class ChunkProcessorInputValidator : AbstractValidator<ChunkProcessorInput>
   {
       public ChunkProcessorInputValidator()
       {
           RuleFor(x => x.ContractVersion)
               .NotEmpty().WithMessage("ContractVersion is required");
           
           RuleFor(x => x.VideoId)
               .NotEmpty().WithMessage("VideoId is required");
           
           RuleFor(x => x.Chunk).NotNull().WithMessage("Chunk is required");
           RuleFor(x => x.Chunk.ChunkId).NotEmpty().WithMessage("Chunk.ChunkId is required");
           RuleFor(x => x.Chunk.StartSec).GreaterThanOrEqualTo(0).WithMessage("Chunk.StartSec must be >= 0");
           RuleFor(x => x.Chunk.EndSec).GreaterThan(x => x.Chunk.StartSec).WithMessage("Chunk.EndSec must be > StartSec");
           
           RuleFor(x => x.Source).NotNull().WithMessage("Source is required");
           RuleFor(x => x.Source.Bucket).NotEmpty().WithMessage("Source.Bucket is required");
           RuleFor(x => x.Source.Key).NotEmpty().WithMessage("Source.Key is required");
           
           RuleFor(x => x.Output).NotNull().WithMessage("Output is required");
           RuleFor(x => x.Output.ManifestBucket).NotEmpty().WithMessage("Output.ManifestBucket is required");
           RuleFor(x => x.Output.ManifestPrefix).NotEmpty().WithMessage("Output.ManifestPrefix is required");
       }
   }
   ```
3. Registrar no DI (`Function.cs` → `ConfigureServices`):
   ```csharp
   services.AddSingleton<IValidator<ChunkProcessorInput>, ChunkProcessorInputValidator>();
   ```

## Formas de Teste
1. **Teste de validação:** criar input válido e invocar validator, verificar que `IsValid = true`
2. **Teste de falha:** criar input com campo vazio (ex.: videoId = ""), verificar que `IsValid = false` e mensagem correta
3. **Teste de range:** criar input com `endSec <= startSec`, verificar falha

## Critérios de Aceite da Subtask
- [ ] FluentValidation 11.9.0 instalado no projeto Application
- [ ] `ChunkProcessorInputValidator` criado com regras: contractVersion, videoId, chunkId não vazios; startSec ≥ 0; endSec > startSec; bucket/key não vazios
- [ ] Validator registrado no DI
- [ ] Projeto compila sem erros
- [ ] Teste quick confirma que input válido passa e inválido falha
