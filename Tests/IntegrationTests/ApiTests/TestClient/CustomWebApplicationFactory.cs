using Infrastructure;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.ApiTests.TestClient;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    private IServiceScope _scope;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services =>
        {
            var serviceProvider = services.BuildServiceProvider();
            _scope = serviceProvider.CreateScope();
            var db = _scope.ServiceProvider.GetRequiredService<EventManagementContext>();
            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        var db = _scope.ServiceProvider.GetRequiredService<EventManagementContext>();
        db.Database.EnsureDeleted();
        base.Dispose(disposing);
    }
}