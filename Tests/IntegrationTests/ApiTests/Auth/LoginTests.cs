using System.Net.Http.Headers;

using Application.Commands.Auth.Login;

using IntegrationTests.ApiTests.TestClient;

namespace IntegrationTests.ApiTests.Auth;

public class LoginTests : BaseApiTests
{
    public LoginTests(WebApiClientTestFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

    [Fact]
    public async Task LoginShouldReturnSuccessIfModelIsValid()
    {
        (_, string email) = await CreateValidUser();

        var model = new LoginUserCommand(email, ValidPassword);
        var client = _fixture.CreateClient();
        (HttpStatusCode statusCode, string token) = await client.LoginUser(model);
        
        StatusCodeIsSuccessful(statusCode);
        
        string.IsNullOrWhiteSpace(token).Should().BeFalse();
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var isCorrectResult = await client.IsCorrectUser();
        StatusCodeIsSuccessful(isCorrectResult);
    }
}