using System.Collections.Generic;
using System.Linq;

using API.Models;

using Application.Models;

using Domain.Enums;

using IntegrationTests.ApiTests.TestClient;

using Newtonsoft.Json;

namespace IntegrationTests.ApiTests.Events.Owner;

public class EventCrud : BaseEventTests
{
    public EventCrud(WebApiClientTestFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

    [Fact]
    public async Task CreateEvent_WithValidModel_ShouldSuccess()
    {
        var @event = new CreateEventRequestModel(Company.Name(), Lorem.Sentence(10), DateTime.UtcNow.AddDays(1),
            (long)TimeSpan.FromHours(1).TotalSeconds, Address.StreetAddress());
        (HttpStatusCode code, string response) = await _ownerClient.CreateEvent(@event);
        StatusCodeIsSuccessful(code);
        var eventId = Guid.Parse(JsonConvert.DeserializeObject<string>(response));

        using var unitOfWork = InitUnitOfWork();
        var dbEvent = unitOfWork.Event.GetById(eventId);
        dbEvent.Id.Should().Be(eventId);
        dbEvent.Title.Should().Be(@event.Title);
        dbEvent.Description.Should().Be(@event.Description);
        dbEvent.StartDate.Should().Be(@event.StartDate);
        dbEvent.Duration.Should().Be(TimeSpan.FromSeconds(@event.Duration));
        dbEvent.Status.Should().Be(EventStatus.InProgress);
        dbEvent.Attendees.Should().BeEmpty();
        dbEvent.RSVPs.Should().BeEmpty();
        dbEvent.Location.Should().Be(@event.Location);
        dbEvent.OwnerId.Should().Be(_ownerId);
    }

    [Fact]
    public async Task CreateEvent_WithInvalidModel_ShouldFail()
    {
        var @event = new CreateEventRequestModel(GenerateRandomWord(101), GenerateRandomWord(501),
            DateTime.UtcNow.AddDays(-11),
            -1, GenerateRandomWord(101));
        (HttpStatusCode code, string response) = await _ownerClient.CreateEvent(@event);
        code.Should().Be(HttpStatusCode.BadRequest);

        _logger.Information(response);
    }

    [Fact]
    public async Task GetEventById_WithValidId_ShouldSuccess()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        (HttpStatusCode code, string response) = await _ownerClient.GetEventById(newEvent.Id);
        StatusCodeIsSuccessful(code);

        var eventResponse = JsonConvert.DeserializeObject<EventQueryModel>(response);

        using var unitOfWork = InitUnitOfWork();
        var dbEvent = unitOfWork.Event.GetById(newEvent.Id);
        dbEvent.Id.Should().Be(newEvent.Id);
        dbEvent.Title.Should().Be(eventResponse.Title);
        dbEvent.Description.Should().Be(eventResponse.Description);
        dbEvent.StartDate.Should().Be(eventResponse.StartDate);
        dbEvent.Duration.Should().Be(eventResponse.Duration);
        dbEvent.Status.Should().Be(EventStatus.InProgress);
        dbEvent.OwnerId.Should().Be(_ownerId);
    }

    [Fact]
    public async Task GetEventById_WithInvalidId_ShouldFail()
    {
        (HttpStatusCode code, string response) = await _ownerClient.GetEventById(Guid.NewGuid());
        code.Should().Be(HttpStatusCode.NotFound);
        _logger.Information(response);
    }

    [Fact]
    public async Task GetAllEvent_AsOwner_WithManyEvents_ShouldSuccess()
    {
        (HttpClient client, string email) = await CreateValidUser();
        var userId = await GetUserId(email);

        using var testService = InitTestService();

        var events = await PerformManyAsync(testService.CreateEvent, userId, 3);

        (HttpStatusCode code, string response) = await client.GetEventForUser(true);
        StatusCodeIsSuccessful(code);

        var eventResponse = JsonConvert.DeserializeObject<IEnumerable<EventQueryModel>>(response);

        foreach (var eventQuery in eventResponse)
        {
            var dbEvent = events.SingleOrDefault(x => x.Id == eventQuery.Id);
            dbEvent.Should().NotBeNull();
            dbEvent.Title.Should().Be(eventQuery.Title);
            dbEvent.Description.Should().Be(eventQuery.Description);
            dbEvent.StartDate.Should().Be(eventQuery.StartDate);
            dbEvent.Duration.Should().Be(eventQuery.Duration);
            dbEvent.Status.Should().Be(EventStatus.InProgress);
            dbEvent.OwnerId.Should().Be(userId);
        }
    }

    [Fact]
    public async Task GetAllEvent_AsOwner_WithoutEvents_ShouldSuccess_AndReturnEmptyCollection()
    {
        (HttpClient client, _) = await CreateValidUser();

        (HttpStatusCode code, string response) = await client.GetEventForUser(true);
        StatusCodeIsSuccessful(code);

        var eventResponse = JsonConvert.DeserializeObject<IEnumerable<EventQueryModel>>(response);

        eventResponse.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteEvent_WithValidId_ShouldSuccess()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        (HttpStatusCode code, string response) = await _ownerClient.DeleteEvent(newEvent.Id);
        StatusCodeIsSuccessful(code);
        string.IsNullOrWhiteSpace(response).Should().BeTrue();
    }

    [Fact]
    public async Task DeleteEvent_WithInvalidId_ShouldFail()
    {
        (HttpStatusCode code, string response) = await _ownerClient.DeleteEvent(Guid.NewGuid());
        code.Should().Be(HttpStatusCode.NotFound);
        _logger.Information(response);
    }

    [Fact]
    public async Task CancelEvent_WithValidData_ShouldSuccess()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        (HttpStatusCode code, string response) = await _ownerClient.CancelEvent(newEvent.Id);
        StatusCodeIsSuccessful(code);
        string.IsNullOrWhiteSpace(response).Should().BeTrue();

        using var unitOfWork = InitUnitOfWork();
        var dbEvent = unitOfWork.Event.GetById(newEvent.Id);
        dbEvent.Status.Should().Be(EventStatus.Cancelled);
    }

    [Fact]
    public async Task CancelEvent_WithNotOwnerUser_ShouldFail()
    {
        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(_ownerId, CancellationToken.None);

        (HttpClient secondClient, _) = await CreateValidUser();

        (HttpStatusCode code, string response) = await secondClient.CancelEvent(newEvent.Id);
        code.Should().Be(HttpStatusCode.NotFound);
        _logger.Information(response);
    }

    [Fact]
    public async Task CancelEvent_WithFinishedEventByDate_ShouldFail()
    {
        using var testService = InitTestService();
        var newEventId = await testService.CreateEventToCorruptData(_ownerId, CancellationToken.None,
            startDate: DateTime.UtcNow.AddDays(-1));

        (HttpStatusCode code, string response) = await _ownerClient.CancelEvent(newEventId);
        code.Should().Be(HttpStatusCode.BadRequest);
        _logger.Information(response);
    }

    [Theory]
    [InlineData(EventStatus.Cancelled)]
    [InlineData(EventStatus.Finished)]
    public async Task CancelEvent_WithCompletedStatuses_ShouldFail(EventStatus completedStatus)
    {
        using var testService = InitTestService();
        var newEventId =
            await testService.CreateEventToCorruptData(_ownerId, CancellationToken.None, status: completedStatus);

        (HttpStatusCode code, string response) = await _ownerClient.CancelEvent(newEventId);
        code.Should().Be(HttpStatusCode.BadRequest);
        _logger.Information(response);
    }
}