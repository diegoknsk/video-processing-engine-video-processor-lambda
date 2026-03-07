using VideoProcessor.Domain.Exceptions;

namespace VideoProcessor.Application.Services;

public class ContractVersionValidator : IContractVersionValidator
{
    private static readonly IReadOnlyList<string> SupportedVersions = ["1.0"];

    public void Validate(string contractVersion)
    {
        if (!SupportedVersions.Contains(contractVersion))
            throw new UnsupportedContractVersionException(contractVersion, SupportedVersions);
    }
}
