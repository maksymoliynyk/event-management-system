using System.Linq;

using MediatR;

namespace ArchitectureTests;

public class ApplicationTests : BaseTestsLogging
{
    public ApplicationTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    [Fact]
    public void AllHandlersShouldBeNamedProperly()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .Should()
            .HaveNameEndingWith("CommandHandler")
            .Or()
            .HaveNameEndingWith("QueryHandler")
            .GetResult();

        if (!result.IsSuccessful)
        {
            var failingHandlers = result.FailingTypes.Select(t => t.FullName);
            _logger.Error(
                "The following handlers do not follow the naming convention: {Handlers}",
                string.Join(", ", failingHandlers)
            );
        }

        result.IsSuccessful.Should().BeTrue();
    }
}