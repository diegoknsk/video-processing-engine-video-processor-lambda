# Subtask 05: Criar Testes Unitários de Serialização/Deserialização

## Descrição
Criar testes unitários que validam deserialização de input, serialização de output, tratamento de campos opcionais, e validação de `contractVersion`.

## Passos de Implementação
1. Criar `tests/VideoProcessor.Tests.Unit/Domain/Models/ChunkProcessorInputTests.cs`:
   - Teste: deserializar JSON completo (todos os campos) para `ChunkProcessorInput`
   - Teste: deserializar JSON mínimo (sem campos opcionais) para `ChunkProcessorInput`
   - Teste: campos opcionais (etag, versionId, executionArn) são null quando ausentes
   - Teste: deserialização falha com JSON inválido (campo obrigatório ausente)
2. Criar `tests/VideoProcessor.Tests.Unit/Domain/Models/ChunkProcessorOutputTests.cs`:
   - Teste: serializar `ChunkProcessorOutput` com status SUCCEEDED contém manifest, não contém error
   - Teste: serializar `ChunkProcessorOutput` com status FAILED contém error, não contém manifest
   - Teste: enum `ProcessingStatus` serializa como "SUCCEEDED"/"FAILED" (string, não número)
   - Teste: campos null são omitidos do JSON (JsonIgnoreCondition.WhenWritingNull)
3. Criar `tests/VideoProcessor.Tests.Unit/Application/Services/ContractVersionValidatorTests.cs`:
   - Teste: validar "1.0" não lança exceção
   - Teste: validar "2.0" lança `UnsupportedContractVersionException`
   - Teste: exceção contém versão recebida e lista de suportadas
4. Usar FluentAssertions para asserções mais legíveis:
   ```csharp
   output.Should().NotBeNull();
   output.Status.Should().Be(ProcessingStatus.SUCCEEDED);
   output.Manifest.Should().NotBeNull();
   output.Error.Should().BeNull();
   ```

## Formas de Teste
1. **Execução de testes:** `dotnet test tests/VideoProcessor.Tests.Unit --filter FullyQualifiedName~Models` executa todos os testes de models
2. **Cobertura:** usar coverlet para verificar cobertura dos models (deve ser 100%)
3. **Validação de JSON:** comparar JSON serializado com estrutura esperada (usando JToken.DeepEquals ou similar)

## Critérios de Aceite da Subtask
- [x] Testes de `ChunkProcessorInput` cobrem: deserialização completa, mínima, campos opcionais, JSON inválido
- [x] Testes de `ChunkProcessorOutput` cobrem: SUCCEEDED com manifest, FAILED com error, enum como string, null omitido
- [x] Testes de `ContractVersionValidator` cobrem: versão válida, versão inválida, mensagem de exceção
- [x] Todos os testes passam: `dotnet test tests/VideoProcessor.Tests.Unit`
- [x] Cobertura de código dos models ≥ 95%
- [x] FluentAssertions utilizado para asserções legíveis
