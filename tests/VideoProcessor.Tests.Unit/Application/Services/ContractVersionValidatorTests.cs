using FluentAssertions;
using VideoProcessor.Application.Services;
using VideoProcessor.Domain.Exceptions;
using Xunit;

namespace VideoProcessor.Tests.Unit.Application.Services;

public class ContractVersionValidatorTests
{
    private readonly ContractVersionValidator _sut = new();

    [Fact]
    public void Validate_VersaoSuportada10_NaoLancaExcecao()
    {
        var act = () => _sut.Validate("1.0");

        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_VersaoNaoSuportada_LancaUnsupportedContractVersionException()
    {
        var act = () => _sut.Validate("2.0");

        act.Should().Throw<UnsupportedContractVersionException>();
    }

    [Fact]
    public void Validate_VersaoNaoSuportada_ExcecaoContemVersaoRecebidaESuportadas()
    {
        var act = () => _sut.Validate("999");

        var ex = act.Should().Throw<UnsupportedContractVersionException>().Subject.Single();
        ex.ReceivedVersion.Should().Be("999");
        ex.SupportedVersions.Should().Contain("1.0");
        ex.Message.Should().Contain("999").And.Contain("1.0");
    }
}
