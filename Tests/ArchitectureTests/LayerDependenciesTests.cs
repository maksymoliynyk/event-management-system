namespace ArchitectureTests;

public class LayerDependenciesTests : BaseTestsLogging
{
    public LayerDependenciesTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact]
    public void DomainLayer_Should_NotHaveDependencies()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAny()
            .GetResult();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOn_InfrastructureLayer()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn(InfrastructureAssembly.GetName().Name)
            .GetResult();
        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void InfrastructureLayer_ShouldNotHaveDependencyOn_ApiLayer()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOn(ApiAssembly.GetName().Name)
            .GetResult();
        result.IsSuccessful.Should().BeTrue();
    }
}