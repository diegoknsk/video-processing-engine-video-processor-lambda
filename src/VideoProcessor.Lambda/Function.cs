using System.Text.Json;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace VideoProcessor.Lambda;

public class Function
{
    public Task<string> FunctionHandler(string input, ILambdaContext context)
    {
        var timestamp = DateTime.UtcNow.ToString("o");
        
        context.Logger.LogInformation($"Hello World invoked at {timestamp}");

        var response = new
        {
            message = "Hello World from Video Processor Lambda",
            version = "1.0.0",
            timestamp = timestamp,
            environment = "dev"
        };

        var responseJson = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return Task.FromResult(responseJson);
    }
}
