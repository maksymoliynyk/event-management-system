using System.Net.Http.Headers;

using Application.Commands.Auth;

using IntegrationTests.ApiTests.TestClient;

namespace IntegrationTests.ApiTests.Auth;

public class LoginTests : BaseApiTests
{
    public LoginTests(WebApiClientTestFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

    [Fact]
    public async Task LoginUser_WithValidCredentials_ShouldSucceed()
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

    [Fact]
    public async Task LoginUser_WithNotExistEmail_ShouldFail()
    {
        await CreateValidUser();

        var model = new LoginUserCommand(Internet.Email(), ValidPassword);
        var client = _fixture.CreateClient();
        (HttpStatusCode statusCode, _) = await client.LoginUser(model);

        statusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task LoginUser_WithWrongPassword_ShouldFail()
    {
        (_, string email) = await CreateValidUser();

        var model = new LoginUserCommand(email, Lorem.GetFirstWord());
        var client = _fixture.CreateClient();
        (HttpStatusCode statusCode, _) = await client.LoginUser(model);

        statusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}