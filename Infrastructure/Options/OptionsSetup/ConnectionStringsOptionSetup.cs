using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Infrastructure.Options.OptionsSetup;

public class ConnectionStringsOptionSetup : IConfigureOptions<ConnectionStringsOption>
{
    private const string SectionName = "ConnectionStrings";
    private readonly IConfiguration _configuration;

    public ConnectionStringsOptionSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(ConnectionStringsOption options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}