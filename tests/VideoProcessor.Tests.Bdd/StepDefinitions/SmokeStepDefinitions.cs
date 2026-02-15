using FluentAssertions;
using TechTalk.SpecFlow;

namespace VideoProcessor.Tests.Bdd.StepDefinitions;

[Binding]
public class SmokeStepDefinitions
{
    [Given(@"o projeto de testes BDD existe")]
    public void GivenOProjetoDeTestesBddExiste()
    {
        // Projeto BDD existe
    }

    [When(@"eu executo os testes")]
    public void WhenEuExecutoOsTestes()
    {
        // Testes são executados
    }

    [Then(@"o teste deve passar")]
    public void ThenOTesteDevePassar()
    {
        true.Should().BeTrue();
    }
}
