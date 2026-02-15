using System.Text.Json;
using Amazon.Lambda.Core;

namespace VideoProcessor.Lambda;

/// <summary>
/// Custom Lambda serializer for JsonDocument input/output.
/// </summary>
public class JsonDocumentLambdaSerializer : ILambdaSerializer
{
    public T Deserialize<T>(Stream requestStream)
    {
        if (typeof(T) != typeof(JsonDocument))
            throw new NotSupportedException("JsonDocumentLambdaSerializer only supports JsonDocument.");

        return (T)(object)JsonDocument.Parse(requestStream);
    }

    public void Serialize<T>(T response, Stream responseStream)
    {
        if (response is not JsonDocument doc)
            throw new NotSupportedException("JsonDocumentLambdaSerializer only supports JsonDocument.");

        using var writer = new Utf8JsonWriter(responseStream, new JsonWriterOptions { Indented = false });
        doc.WriteTo(writer);
        writer.Flush();
    }
}
