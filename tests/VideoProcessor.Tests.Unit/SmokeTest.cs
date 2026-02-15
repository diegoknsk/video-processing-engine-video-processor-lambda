using FluentAssertions;

namespace VideoProcessor.Tests.Unit;

public class SmokeTest
{
    [Fact]
    public void Projeto_de_testes_unitarios_esta_configurado()
    {
        true.Should().BeTrue();
    }
}
