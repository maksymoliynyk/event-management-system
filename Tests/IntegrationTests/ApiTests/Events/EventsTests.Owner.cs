using System.Collections.Generic;
using System.Linq;

using API.Models;

using Application.Models;

using Domain.Enums;

using IntegrationTests.ApiTests.TestClient;

using Newtonsoft.Json;

namespace IntegrationTests.ApiTests.Events;

public partial class EventsTests : BaseApiTests
{
    public EventsTests(WebApiClientTestFixture fixture, ITestOutputHelper outputHelper) : base(fixture, outputHelper)
    {
    }

    [Fact]
    public async Task CreateValidEventShouldBeSuccessful()
    {
        (HttpClient client, string email) = await CreateValidUser();
        var userId = await GetUserId(email);

        var @event = new CreateEventRequestModel(Company.Name(), Lorem.Sentence(10), DateTime.UtcNow.AddDays(1),
            (long)TimeSpan.FromHours(1).TotalSeconds, Address.StreetAddress());
        (HttpStatusCode code, Guid eventId) = await client.CreateEvent(@event);
        StatusCodeIsSuccessful(code);

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
        dbEvent.OwnerId.Should().Be(userId);
    }

    [Fact]
    public async Task GetEventByIdShouldReturnCorrectEvent()
    {
        (HttpClient client, string email) = await CreateValidUser();
        var userId = await GetUserId(email);

        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(userId, CancellationToken.None);

        (HttpStatusCode code, string response) = await client.GetEventById(newEvent.Id);
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
        dbEvent.OwnerId.Should().Be(userId);
    }

    [Fact]
    public async Task GetAllEventAsOwnerShouldReturnCorrectEvents()
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
    public async Task DeleteEventShouldBeSuccessful()
    {
        (HttpClient client, string email) = await CreateValidUser();
        var userId = await GetUserId(email);

        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(userId, CancellationToken.None);

        (HttpStatusCode code, string response) = await client.DeleteEvent(newEvent.Id);
        StatusCodeIsSuccessful(code);
        string.IsNullOrWhiteSpace(response).Should().BeTrue();

        using var unitOfWork = InitUnitOfWork();
        var dbEvent = unitOfWork.Event.GetById(newEvent.Id);
        dbEvent.Should().BeNull();
    }

    [Fact]
    public async Task CancelEventShouldBeSuccessful()
    {
        (HttpClient client, string email) = await CreateValidUser();
        var userId = await GetUserId(email);

        using var testService = InitTestService();
        var newEvent = await testService.CreateEvent(userId, CancellationToken.None);

        (HttpStatusCode code, string response) = await client.CancelEvent(newEvent.Id);
        StatusCodeIsSuccessful(code);
        string.IsNullOrWhiteSpace(response).Should().BeTrue();

        using var unitOfWork = InitUnitOfWork();
        var dbEvent = unitOfWork.Event.GetById(newEvent.Id);
        dbEvent.Status.Should().Be(EventStatus.Cancelled);
    }
}