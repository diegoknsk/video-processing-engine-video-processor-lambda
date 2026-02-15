# Subtask 03: Implementar Versionamento e Exceções de Versão Não Suportada

## Descrição
Criar exceção customizada `UnsupportedContractVersionException`, implementar lógica de validação de `contractVersion` no início do handler, e definir versões suportadas (inicialmente apenas "1.0").

## Passos de Implementação
1. Criar `src/VideoProcessor.Domain/Exceptions/UnsupportedContractVersionException.cs`:
   ```csharp
   public class UnsupportedContractVersionException : Exception
   {
       public string ReceivedVersion { get; }
       public IReadOnlyList<string> SupportedVersions { get; }
       
       public UnsupportedContractVersionException(string receivedVersion, IReadOnlyList<string> supportedVersions)
           : base($"Contract version '{receivedVersion}' is not supported. Supported versions: {string.Join(", ", supportedVersions)}")
       {
           ReceivedVersion = receivedVersion;
           SupportedVersions = supportedVersions;
       }
   }
   ```
2. Criar `src/VideoProcessor.Application/Services/ContractVersionValidator.cs`:
   ```csharp
   public interface IContractVersionValidator
   {
       void Validate(string contractVersion);
   }
   
   public class ContractVersionValidator : IContractVersionValidator
   {
       private static readonly IReadOnlyList<string> SupportedVersions = new[] { "1.0" };
       
       public void Validate(string contractVersion)
       {
           if (!SupportedVersions.Contains(contractVersion))
           {
               throw new UnsupportedContractVersionException(contractVersion, SupportedVersions);
           }
       }
   }
   ```
3. Registrar `IContractVersionValidator` no DI (`Function.cs` → `ConfigureServices`):
   ```csharp
   services.AddSingleton<IContractVersionValidator, ContractVersionValidator>();
   ```
4. No handler, após deserializar input, validar versão:
   ```csharp
   var input = JsonSerializer.Deserialize<ChunkProcessorInput>(inputJson);
   _versionValidator.Validate(input.ContractVersion);
   ```
5. Documentar versões suportadas em `README.md` ou `CONTRACTS.md`

## Formas de Teste
1. **Teste unitário:** validar que `ContractVersionValidator.Validate("1.0")` não lança exceção
2. **Teste unitário:** validar que `Validate("2.0")` lança `UnsupportedContractVersionException`
3. **Teste de integração:** invocar handler com `contractVersion: "999"` e verificar exceção

## Critérios de Aceite da Subtask
- [x] `UnsupportedContractVersionException` criada com propriedades `ReceivedVersion` e `SupportedVersions`
- [x] `IContractVersionValidator` e implementação criados
- [x] Versão "1.0" definida como suportada
- [x] Validador registrado no DI
- [x] Testes unitários cobrem: versão válida (não lança exceção), versão inválida (lança exceção com mensagem clara)
- [x] Documentação lista versões suportadas
