namespace VideoProcessor.Domain.Exceptions;

public class UnsupportedContractVersionException(string receivedVersion, IReadOnlyList<string> supportedVersions)
    : Exception($"Contract version '{receivedVersion}' is not supported. Supported versions: {string.Join(", ", supportedVersions)}")
{
    public string ReceivedVersion { get; } = receivedVersion;
    public IReadOnlyList<string> SupportedVersions { get; } = supportedVersions;
}
