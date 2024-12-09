using API;

using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.ApiTests.TestClient;

public class WebApiClientTestFixture : IDisposable
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    public WebApiClientTestFixture()
    {
        _factory = new CustomWebApplicationFactory<Program>();
    }

    public HttpClient CreateClient() => _factory.CreateClient();
    public T GetService<T>() => _factory.Services.GetRequiredService<T>();

    public void Dispose()
    {
        _factory.Dispose();
    }
}