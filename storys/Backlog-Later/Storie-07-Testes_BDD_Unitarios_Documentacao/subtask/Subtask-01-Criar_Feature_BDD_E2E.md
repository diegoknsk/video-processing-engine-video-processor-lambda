# Subtask 01: Criar Feature BDD End-to-End e Step Definitions

## Descrição
Criar feature SpecFlow `ProcessChunk.feature` com cenários que validam fluxo completo de processamento de chunk (sucesso, idempotência, validação), e implementar step definitions correspondentes usando mocks de S3Service.

## Passos de Implementação
1. Criar `tests/VideoProcessor.Tests.Bdd/Features/ProcessChunk.feature`:
   ```gherkin
   Feature: Process Video Chunk
     Como componente de processamento de vídeo
     Quero processar chunks individuais de vídeo
     Para gerar manifestos e frames no pipeline distribuído
   
   Scenario: Processar chunk novo com sucesso
     Given que recebo um input válido para chunk "chunk-0" do vídeo "video-123"
     And o marker de conclusão "done.json" não existe no S3
     When eu processar o chunk
     Then o status deve ser "SUCCEEDED"
     And o manifest deve ser gravado no S3
     And o marker de conclusão deve ser gravado no S3
     And a resposta deve conter o caminho do manifest
   
   Scenario: Chunk já processado (idempotência)
     Given que recebo um input válido para chunk "chunk-0" do vídeo "video-123"
     And o marker de conclusão "done.json" existe no S3
     When eu processar o chunk
     Then o status deve ser "SUCCEEDED"
     And o manifest não deve ser gravado novamente no S3
     And a resposta deve conter o caminho do manifest existente
   
   Scenario: Input inválido (validação falha)
     Given que recebo um input com videoId vazio
     When eu processar o chunk
     Then o status deve ser "FAILED"
     And o erro deve ser do tipo "ValidationError"
     And o erro deve ser marcado como não retryable
   ```
2. Criar `tests/VideoProcessor.Tests.Bdd/StepDefinitions/ProcessChunkSteps.cs`:
   ```csharp
   [Binding]
   public class ProcessChunkSteps
   {
       private readonly ScenarioContext _context;
       private ChunkProcessorInput _input = null!;
       private ChunkProcessorOutput _output = null!;
       private Mock<IS3Service> _s3Mock = null!;
       private ProcessChunkUseCase _useCase = null!;
       
       public ProcessChunkSteps(ScenarioContext context)
       {
           _context = context;
       }
       
       [Given(@"que recebo um input válido para chunk ""(.*)"" do vídeo ""(.*)""")]
       public void GivenInputValido(string chunkId, string videoId)
       {
           _input = new ChunkProcessorInput(
               ContractVersion: "1.0",
               VideoId: videoId,
               Chunk: new ChunkInfo(chunkId, 0.0, 10.0),
               Source: new SourceInfo("bucket", "key"),
               Output: new OutputConfig("out-bucket", "prefix")
           );
           
           _s3Mock = new Mock<IS3Service>();
           var prefixBuilder = new PrefixBuilder();
           var validator = new ChunkProcessorInputValidator();
           var logger = Mock.Of<ILogger<ProcessChunkUseCase>>();
           
           _useCase = new ProcessChunkUseCase(_s3Mock.Object, prefixBuilder, validator, logger);
       }
       
       [Given(@"o marker de conclusão ""done\.json"" não existe no S3")]
       public void GivenDoneNaoExiste()
       {
           _s3Mock.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(false);
       }
       
       [Given(@"o marker de conclusão ""done\.json"" existe no S3")]
       public void GivenDoneExiste()
       {
           _s3Mock.Setup(s => s.ExistsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(true);
       }
       
       [Given(@"que recebo um input com videoId vazio")]
       public void GivenInputInvalido()
       {
           _input = new ChunkProcessorInput(
               ContractVersion: "1.0",
               VideoId: "", // Inválido
               Chunk: new ChunkInfo("chunk-0", 0.0, 10.0),
               Source: new SourceInfo("bucket", "key"),
               Output: new OutputConfig("out-bucket", "prefix")
           );
           
           _s3Mock = new Mock<IS3Service>();
           var prefixBuilder = new PrefixBuilder();
           var validator = new ChunkProcessorInputValidator();
           var logger = Mock.Of<ILogger<ProcessChunkUseCase>>();
           
           _useCase = new ProcessChunkUseCase(_s3Mock.Object, prefixBuilder, validator, logger);
       }
       
       [When(@"eu processar o chunk")]
       public async Task WhenProcessar()
       {
           try
           {
               _output = await _useCase.ExecuteAsync(_input, CancellationToken.None);
           }
           catch (ChunkValidationException ex)
           {
               // Capturar para validar erro
               _context["ValidationException"] = ex;
               _output = new ChunkProcessorOutput(
                   ChunkId: _input.Chunk.ChunkId,
                   Status: ProcessingStatus.FAILED,
                   FramesCount: 0,
                   Error: new ErrorInfo("ValidationError", ex.Message, false)
               );
           }
       }
       
       [Then(@"o status deve ser ""(.*)""")]
       public void ThenStatus(string expectedStatus)
       {
           _output.Status.ToString().Should().Be(expectedStatus);
       }
       
       [Then(@"o manifest deve ser gravado no S3")]
       public void ThenManifestGravado()
       {
           _s3Mock.Verify(s => s.PutJsonAsync(
               It.IsAny<string>(),
               It.Is<string>(k => k.Contains("manifest.json")),
               It.IsAny<object>(),
               It.IsAny<CancellationToken>()),
               Times.Once);
       }
       
       [Then(@"o marker de conclusão deve ser gravado no S3")]
       public void ThenDoneGravado()
       {
           _s3Mock.Verify(s => s.PutJsonAsync(
               It.IsAny<string>(),
               It.Is<string>(k => k.Contains("done.json")),
               It.IsAny<object>(),
               It.IsAny<CancellationToken>()),
               Times.Once);
       }
       
       [Then(@"a resposta deve conter o caminho do manifest")]
       public void ThenRespostaComManifest()
       {
           _output.Manifest.Should().NotBeNull();
           _output.Manifest!.Key.Should().Contain("manifest.json");
       }
       
       [Then(@"o manifest não deve ser gravado novamente no S3")]
       public void ThenManifestNaoGravado()
       {
           _s3Mock.Verify(s => s.PutJsonAsync(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<object>(),
               It.IsAny<CancellationToken>()),
               Times.Never);
       }
       
       [Then(@"a resposta deve conter o caminho do manifest existente")]
       public void ThenRespostaComManifestExistente()
       {
           _output.Manifest.Should().NotBeNull();
       }
       
       [Then(@"o erro deve ser do tipo ""(.*)""")]
       public void ThenErroTipo(string expectedType)
       {
           _output.Error.Should().NotBeNull();
           _output.Error!.Type.Should().Be(expectedType);
       }
       
       [Then(@"o erro deve ser marcado como não retryable")]
       public void ThenErroNaoRetryable()
       {
           _output.Error.Should().NotBeNull();
           _output.Error!.Retryable.Should().BeFalse();
       }
   }
   ```
3. Adicionar using statements necessários: `using Moq;`, `using FluentAssertions;`, etc.

## Formas de Teste
1. **Execução:** `dotnet test tests/VideoProcessor.Tests.Bdd` deve executar e passar os 3 cenários
2. **Relatório SpecFlow:** verificar relatório de execução (se disponível)
3. **Cobertura:** cenários cobrem casos principais do fluxo

## Critérios de Aceite da Subtask
- [ ] Feature `ProcessChunk.feature` criada com 3 cenários: sucesso, idempotência, validação falha
- [ ] Step definitions implementados para todos os steps
- [ ] Mocks configurados corretamente (S3Service, PrefixBuilder, Validator)
- [ ] Todos os cenários passam: `dotnet test tests/VideoProcessor.Tests.Bdd`
- [ ] Assertions usam FluentAssertions
- [ ] Teste BDD valida fluxo end-to-end (input → use case → output)
