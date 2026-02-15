using System.Text.Json;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;

[assembly: LambdaSerializer(typeof(VideoProcessor.Lambda.JsonDocumentLambdaSerializer))]

namespace VideoProcessor.Lambda;

public class Function
{
    private readonly IServiceProvider _serviceProvider;

    public Function()
    {
        _serviceProvider = ConfigureServices();
    }

    public Task<JsonDocument> FunctionHandler(JsonDocument input, ILambdaContext context)
    {
        context.Logger.LogInformation($"Input received: {input.RootElement.GetRawText()}");

        var response = new Dictionary<string, string>
        {
            ["status"] = "SUCCEExxDED !!!",
            ["message"] = "Teste realizado com sucessoMock xxx from bootstrap"
        };

        var result = JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(response));
        return Task.FromResult(result);
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Application services (preparar para próximas stories)
        // Infra services (preparar para próximas stories)

        return services.BuildServiceProvider();
    }
}
