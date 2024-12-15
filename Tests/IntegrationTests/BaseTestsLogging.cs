using Serilog;

namespace IntegrationTests;

public abstract class BaseTestsLogging
{
    protected readonly ILogger _logger;

    public BaseTestsLogging(ITestOutputHelper outputHelper)
    {
        _logger = new LoggerConfiguration()
            .WriteTo
            .XunitTestOutput(outputHelper)
            .CreateLogger()
            .ForContext<BaseTestsLogging>();
    }
}