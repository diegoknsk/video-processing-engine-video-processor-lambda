using FluentAssertions;
using TechTalk.SpecFlow;
using VideoProcessor.Application.UseCases;
using VideoProcessor.Domain.Exceptions;
using VideoProcessor.Domain.Models;
using VideoProcessor.Domain.Ports;
using VideoProcessor.Domain.Services;

namespace VideoProcessor.Tests.Bdd.StepDefinitions;

[Binding]
public class SimulacaoErroDuracaoStepDefinitions
{
    private ChunkProcessorInput? _input;
    private Exception? _capturedException;
    private int _triggerValue;

    // Stub que simula extrator detectando vídeo com 1303 segundos
    private sealed class ExtractorSimulacao1303 : IVideoFrameExtractor
    {
        public Task<FrameExtractionResult> ExtractFramesAsync(
            string videoPath, int intervalSeconds, string outputFolder,
            int? startTimeSeconds = null, int? endTimeSeconds = null)
            => throw new VideoDurationSimulationException(VideoDurationSimulationException.TriggerDurationSeconds);
    }

    // Stub de S3 que simula download bem-sucedido
    private sealed class S3StorageStub : IS3VideoStorage
    {
        public Task<string> DownloadToTempAsync(string bucket, string key, string destPath, CancellationToken ct = default)
            => Task.FromResult(destPath);

        public Task<IReadOnlyList<string>> UploadFramesAsync(string bucket, string prefix, IEnumerable<string> framePaths, CancellationToken ct = default)
            => Task.FromResult<IReadOnlyList<string>>([]);
    }

    [Given(@"um input válido de processamento de chunk")]
    public void GivenUmInputValidoDeProcessamentoDeChunk()
    {
        _input = new ChunkProcessorInput(
            ContractVersion: "1.0",
            VideoId: "video-simulacao",
            Chunk: new ChunkInfo("chunk-001", 0, 1303, 1),
            Source: new SourceInfo("bucket-src", "videos/simulacao.mp4"),
            Output: new OutputConfig("bucket-out", "frames/", FramesBucket: "bucket-out", FramesPrefix: "frames/"));
    }

    [When(@"o extrator detecta que o vídeo tem 1303 segundos de duração")]
    public async Task WhenOExtratorDetectaQueOVideoTem1303SegundosDeDuracao()
    {
        var sut = new ProcessChunkUseCase(new ExtractorSimulacao1303(), new S3StorageStub());

        try
        {
            await sut.ExecuteAsync(_input!);
        }
        catch (Exception ex)
        {
            _capturedException = ex;
        }
    }

    [Then(@"o UseCase deve propagar a VideoDurationSimulationException sem capturá-la")]
    public void ThenOUseCaseDevePropagarAVideoDurationSimulationExceptionSemCapturala()
    {
        _capturedException.Should().NotBeNull();
        _capturedException.Should().BeOfType<VideoDurationSimulationException>();
    }

    [Then(@"a mensagem da exceção deve indicar que é uma simulação intencional")]
    public void ThenAMensagemDaExcecaoDeveIndicarQueEUmaSimulacaoIntencional()
    {
        _capturedException!.Message.Should().Contain("SIMULAÇÃO");
    }

    [Given(@"a regra de negócio de simulação está configurada")]
    public void GivenARegraDeNegocioDeSimulacaoEstaConfigurada()
    {
        // A constante é parte da regra de domínio
    }

    [When(@"consulto o valor de disparo da simulação")]
    public void WhenConsultoOValorDeDisparoDaSimulacao()
    {
        _triggerValue = VideoDurationSimulationException.TriggerDurationSeconds;
    }

    [Then(@"o valor deve ser igual a 1303")]
    public void ThenOValorDeveSerIgualA1303()
    {
        _triggerValue.Should().Be(1303);
    }
}
