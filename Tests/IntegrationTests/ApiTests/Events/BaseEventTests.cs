using IntegrationTests.ApiTests.TestClient;

namespace IntegrationTests.ApiTests.Events;

public abstract class BaseEventTests : BaseApiTests
{
    protected HttpClient _ownerClient;
    protected string _ownerEmail;
    protected Guid _ownerId;

    protected HttpClient _inviteeClient;
    protected string _inviteeEmail;
    protected Guid _inviteeId;


    protected BaseEventTests(WebApiClientTestFixture fixture, ITestOutputHelper outputHelper) : base(fixture,
        outputHelper)
    {
        PrepareData().Wait();
    }

    private async Task PrepareData()
    {
        (HttpClient client, string email) = await CreateValidUser();
        _ownerEmail = email;
        _ownerClient = client;
        _ownerId = await GetUserId(_ownerEmail);

        (HttpClient inviteeClient, string inviteeEmail) = await CreateValidUser();
        _inviteeEmail = inviteeEmail;
        _inviteeClient = inviteeClient;
        _inviteeId = await GetUserId(_inviteeEmail);
    }
}