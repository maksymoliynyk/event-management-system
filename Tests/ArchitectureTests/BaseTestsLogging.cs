using Serilog;

namespace ArchitectureTests;

public abstract class BaseTestsLogging
{
    protected readonly ILogger _logger;

    protected BaseTestsLogging(ITestOutputHelper outputHelper)
    {
        _logger = new LoggerConfiguration()
            .WriteTo
            .XunitTestOutput(outputHelper)
            .CreateLogger()
            .ForContext<BaseTestsLogging>();
    }
}