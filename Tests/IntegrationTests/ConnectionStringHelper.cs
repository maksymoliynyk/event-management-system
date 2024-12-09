using Infrastructure.Options;

using Microsoft.Extensions.Configuration;

namespace IntegrationTests;

public class ConnectionStringHelper
{
    public ConnectionStringsOption Options { get; }

    public ConnectionStringHelper()
    {
        const string environment = "Test";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();

        Options = configuration.GetSection("ConnectionStrings").Get<ConnectionStringsOption>()!;
    }
}