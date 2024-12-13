using System.Collections.Generic;
using System.Linq;

using Application.Models;

using Domain.Enums;

using IntegrationTests.ApiTests.TestClient;

using Newtonsoft.Json;

namespace IntegrationTests.ApiTests.Events.Owner;

public class EventInvitesTests : BaseEventTests
{
    public EventInvitesTests(WebApiClientTestFixture fixture, ITestOutputHelper outputHelper) : base(fixture,
        outputHelper)
    {
    }

    [Fact]
    public async Task SendRsvp_WithExistedNonInvitedUser_ShouldSuccess()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        var response = await _ownerClient.InviteUser(newEvent.Id, _inviteeEmail);
        StatusCodeIsSuccessful(response.Code);

        using var unitOfWork = InitUnitOfWork();
        var dbEvent = unitOfWork.Event.GetById(newEvent.Id);
        dbEvent.Should().NotBeNull();
        dbEvent.RSVPs.Should().NotBeNullOrEmpty();
        dbEvent.RSVPs.Should().HaveCount(1);
        dbEvent.RSVPs.First().UserId.Should().Be(_inviteeId);
    }

    [Fact]
    public async Task SendRsvp_WithNotExistedUser_ShouldFail()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        var response = await _ownerClient.InviteUser(newEvent.Id, Internet.Email());
        response.Code.Should().Be(HttpStatusCode.NotFound);
        _logger.Information(response.Response);
    }

    [Fact]
    public async Task SendRsvp_WithOwnerEmail_ShouldFail()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        var response = await _ownerClient.InviteUser(newEvent.Id, _ownerEmail);
        response.Code.Should().Be(HttpStatusCode.BadRequest);
        _logger.Information(response.Response);
    }

    [Fact]
    public async Task SendRsvp_WithAlreadyInvitedUser_ShouldFail()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        var response = await _ownerClient.InviteUser(newEvent.Id, _inviteeEmail);
        StatusCodeIsSuccessful(response.Code);

        using var unitOfWork = InitUnitOfWork();
        var dbEvent = unitOfWork.Event.GetById(newEvent.Id);
        dbEvent.Should().NotBeNull();
        dbEvent.RSVPs.Should().NotBeNullOrEmpty();
        dbEvent.RSVPs.Should().HaveCount(1);
        dbEvent.RSVPs.First().UserId.Should().Be(_inviteeId);

        var secondResponse = await _ownerClient.InviteUser(newEvent.Id, _inviteeEmail);
        secondResponse.Code.Should().Be(HttpStatusCode.BadRequest);
        _logger.Information(secondResponse.Response);
    }

    [Fact]
    public async Task GetRsvps_ByEvent_WithData_ShouldSuccess()
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

        var getResponse = await _ownerClient.GetRsvpsForEvent(newEvent.Id);
        StatusCodeIsSuccessful(getResponse.Code);

        var rsvps = JsonConvert.DeserializeObject<IEnumerable<RSVPQueryModel>>(getResponse.Response);
        var rsvp = rsvps.First();
        rsvp.Should().NotBeNull();
        rsvp.EventId.Should().Be(newEvent.Id);
        rsvp.InviteeEmail.Should().Be(_inviteeEmail);
    }

    [Fact]
    public async Task GetRsvps_ByEvent_WithoutData_ShouldSuccess()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        var getResponse = await _ownerClient.GetRsvpsForEvent(newEvent.Id);
        StatusCodeIsSuccessful(getResponse.Code);

        var rsvps = JsonConvert.DeserializeObject<IEnumerable<RSVPQueryModel>>(getResponse.Response);
        rsvps.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetAttendees_ByEvent_WithData_ShouldSuccess()
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

        var getResponse = await _ownerClient.GetAttendeesForEvent(newEvent.Id);
        StatusCodeIsSuccessful(getResponse.Code);

        var attendees = JsonConvert.DeserializeObject<IEnumerable<AttendeeQueryModel>>(getResponse.Response);
        var attendee = attendees.First();
        attendee.Should().NotBeNull();
        attendee.EventId.Should().Be(newEvent.Id);
        attendee.AttendeeEmail.Should().Be(_inviteeEmail);
    }

    [Fact]
    public async Task GetAttendees_ByEvent_WithoutData_ShouldSuccess()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        var getResponse = await _ownerClient.GetAttendeesForEvent(newEvent.Id);
        StatusCodeIsSuccessful(getResponse.Code);

        var attendees = JsonConvert.DeserializeObject<IEnumerable<AttendeeQueryModel>>(getResponse.Response);
        attendees.Should().BeNullOrEmpty();
    }
}