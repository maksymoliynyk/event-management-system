using Application.Commands.Auth.Register;

using IntegrationTests.ApiTests.TestClient;

namespace IntegrationTests.ApiTests.Auth;

public class RegisterTests : BaseApiTests
{
    public RegisterTests(WebApiClientTestFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

    // Todo: Adjust test cases if password settings would be changed
    [Fact]
    public async Task RegisterUser_WithWeakPassword_ShouldFail()
    {
        var client = _fixture.CreateClient();
        var weakPasswordModel = new RegisterUserCommand(Internet.Email(), "John", "Doe", "123");

        (HttpStatusCode code, string response) = await client.RegisterUser(weakPasswordModel);

        code.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WithDuplicateEmail_ShouldFail()
    {
        var client = _fixture.CreateClient();
        var registerModel = new RegisterUserCommand(Internet.Email(), "John", "Doe", "password123");

        var firstResponse = await client.RegisterUser(registerModel);
        var secondResponse = await client.RegisterUser(registerModel);

        StatusCodeIsSuccessful(firstResponse.Code);
        secondResponse.Code.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WithInvalidEmail_ShouldFail()
    {
        var client = _fixture.CreateClient();
        var email = Internet.Email();
        email = email.Replace("@", "");
        var registerModel = new RegisterUserCommand(email, "John", "Doe", "password123");

        var response = await client.RegisterUser(registerModel);

        response.Code.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterUser_WithValidPassword_ShouldSucceed()
    {
        var client = _fixture.CreateClient();
        var validPasswordModel = new RegisterUserCommand(Internet.Email(), "John", "Doe", "password123");

        var response = await client.RegisterUser(validPasswordModel);

        StatusCodeIsSuccessful(response.Code);
    }

}