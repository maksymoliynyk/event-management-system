using System.Collections.Generic;
using System.Linq;

using Application.Models;

using Domain.Enums;

using IntegrationTests.ApiTests.TestClient;

using Newtonsoft.Json;

namespace IntegrationTests.ApiTests.Events.Invitee;

public class EventInviteeTests : BaseEventTests
{
    public EventInviteeTests(WebApiClientTestFixture fixture, ITestOutputHelper outputHelper) : base(fixture,
        outputHelper)
    {
    }

    [Fact]
    public async Task GetEventById_WithNotOwnerAndNotInvited_ShouldFail()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        (HttpStatusCode code, string response) = await _inviteeClient.GetEventById(newEvent.Id);
        code.Should().Be(HttpStatusCode.NotFound);
        _logger.Information(response);
    }

    [Fact]
    public async Task GetEventById_WithInvitedUser_ShouldSuccess()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        var inviteResponse = await _ownerClient.InviteUser(newEvent.Id, _inviteeEmail);
        StatusCodeIsSuccessful(inviteResponse.Code);

        (HttpStatusCode code, string response) = await _inviteeClient.GetEventById(newEvent.Id);
        StatusCodeIsSuccessful(code);
        var @event = JsonConvert.DeserializeObject<EventQueryModel>(response);
        @event.Should().NotBeNull();
        @event.Id.Should().Be(newEvent.Id);
    }

    [Fact]
    public async Task GetAttendees_ByEvent_WithNotOwner_ShouldSuccess_AndBeEmpty()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        var inviteResponse = await _ownerClient.InviteUser(newEvent.Id, _inviteeEmail);
        StatusCodeIsSuccessful(inviteResponse.Code);

        using var unitOfWork = InitUnitOfWork();
        var dbEvent = unitOfWork.Event.GetById(newEvent.Id);
        dbEvent.Should().NotBeNull();
        dbEvent.RSVPs.Should().NotBeNullOrEmpty();
        dbEvent.RSVPs.Should().HaveCount(1);
        dbEvent.RSVPs.First().UserId.Should().Be(_inviteeId);

        dbEvent.RespondToRsvp(RSVPStatus.Accepted, _inviteeId);
        await unitOfWork.SaveAsync(CancellationToken.None);

        (HttpClient client, _) = await CreateValidUser();

        var getResponse = await client.GetAttendeesForEvent(newEvent.Id);
        StatusCodeIsSuccessful(getResponse.Code);

        var attendees = JsonConvert.DeserializeObject<IEnumerable<AttendeeQueryModel>>(getResponse.Response);
        attendees.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetRSVPs_ByEvent_WithNotOwner_ShouldSuccess_AndBeEmpty()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        var inviteResponse = await _ownerClient.InviteUser(newEvent.Id, _inviteeEmail);
        StatusCodeIsSuccessful(inviteResponse.Code);

        (HttpClient client, _) = await CreateValidUser();

        var getResponse = await client.GetRsvpsForEvent(newEvent.Id);
        StatusCodeIsSuccessful(getResponse.Code);

        var rsvps = JsonConvert.DeserializeObject<IEnumerable<AttendeeQueryModel>>(getResponse.Response);
        rsvps.Should().BeNullOrEmpty();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task RespondRSVP_WithInvitedUser_ShouldSuccess(bool isAccepted)
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        var inviteResponse = await _ownerClient.InviteUser(newEvent.Id, _inviteeEmail);
        StatusCodeIsSuccessful(inviteResponse.Code);

        var respondResponse = await _inviteeClient.RespondToRSVP(newEvent.Id, isAccepted);
        StatusCodeIsSuccessful(respondResponse.Code);

        using var unitOfWork = InitUnitOfWork();
        var dbEvent = unitOfWork.Event.GetById(newEvent.Id);
        var attendee = dbEvent.Attendees.FirstOrDefault(a => a.AttendeeId == _inviteeId);
        if (isAccepted)
        {
            attendee.Should().NotBeNull();
        }
        else
        {
            attendee.Should().BeNull();
        }
    }

    [Fact]
    public async Task RespondRSVP_WithAlreadyResponded_ShouldFail()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        var inviteResponse = await _ownerClient.InviteUser(newEvent.Id, _inviteeEmail);
        StatusCodeIsSuccessful(inviteResponse.Code);

        var respondResponse = await _inviteeClient.RespondToRSVP(newEvent.Id, true);
        StatusCodeIsSuccessful(respondResponse.Code);

        var secondResponse = await _inviteeClient.RespondToRSVP(newEvent.Id, true);
        secondResponse.Code.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RespondRSVP_WithNotInvited_ShouldFail()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        var inviteResponse = await _ownerClient.InviteUser(newEvent.Id, _inviteeEmail);
        StatusCodeIsSuccessful(inviteResponse.Code);

        (HttpClient client, _) = await CreateValidUser();

        var respondResponse = await client.RespondToRSVP(newEvent.Id, true);
        respondResponse.Code.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetRSVPs_WithData_ShouldSuccess()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        var inviteResponse = await _ownerClient.InviteUser(newEvent.Id, _inviteeEmail);
        StatusCodeIsSuccessful(inviteResponse.Code);

        var getResponse = await _inviteeClient.GetRsvpsForUser();
        StatusCodeIsSuccessful(getResponse.Code);
        var rsvps = JsonConvert.DeserializeObject<IEnumerable<RSVPQueryModel>>(getResponse.Response);
        rsvps.Should().NotBeNullOrEmpty();
        rsvps.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetRSVPs_WithoutData_ShouldSuccess_AndReturnEmptyList()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        var inviteResponse = await _ownerClient.InviteUser(newEvent.Id, _inviteeEmail);
        StatusCodeIsSuccessful(inviteResponse.Code);

        (HttpClient client, _) = await CreateValidUser();

        var getResponse = await client.GetRsvpsForUser();
        StatusCodeIsSuccessful(getResponse.Code);
        var rsvps = JsonConvert.DeserializeObject<IEnumerable<RSVPQueryModel>>(getResponse.Response);
        rsvps.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetEvents_WithoutData_ShouldSuccess_AndReturnEmptyList()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);
        await testService.CreateEvent(_inviteeId, CancellationToken.None);

        var inviteResponse = await _ownerClient.InviteUser(newEvent.Id, _inviteeEmail);
        StatusCodeIsSuccessful(inviteResponse.Code);

        var getResponse = await _inviteeClient.GetEventForUser();
        StatusCodeIsSuccessful(getResponse.Code);
        var events = JsonConvert.DeserializeObject<IEnumerable<EventQueryModel>>(getResponse.Response);
        events.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetEvents_WithData_ShouldSuccess()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);
        await testService.CreateEvent(_inviteeId, CancellationToken.None);

        var inviteResponse = await _ownerClient.InviteUser(newEvent.Id, _inviteeEmail);
        StatusCodeIsSuccessful(inviteResponse.Code);

        var respondResponse = await _inviteeClient.RespondToRSVP(newEvent.Id, true);
        StatusCodeIsSuccessful(respondResponse.Code);

        var getResponse = await _inviteeClient.GetEventForUser();
        StatusCodeIsSuccessful(getResponse.Code);
        var events = JsonConvert.DeserializeObject<IEnumerable<EventQueryModel>>(getResponse.Response);
        events.Should().NotBeNullOrEmpty();
        events.Should().HaveCount(1);
    }
}