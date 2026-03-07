using System.Text.Json.Serialization;

namespace VideoProcessor.Domain.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProcessingStatus
{
    SUCCEEDED,
    FAILED
}
