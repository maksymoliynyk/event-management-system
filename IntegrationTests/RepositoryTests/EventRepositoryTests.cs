using System;
using System.Threading.Tasks;

using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Repositories;

using IntegrationTests.RepositoryTests.Helpers;

using Shouldly;
using Faker;
using Domain.DbContexts;
using Microsoft.EntityFrameworkCore;
using Contracts.Models.Statuses;
using Domain.Exceptions;

namespace IntegrationTests.RepositoryTests
{
    public class EventRepositoryTests
    {
        private readonly IRepositoryManager _repositoryManager;
        public EventRepositoryTests()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            _repositoryManager = new RepositoryManager(TestContext.GetEventManagementContext());
        }
        [Fact]
        public async Task CreateEventShouldCreateEvent()
        {
            // Arrange
            using EventManagementContext context = TestContext.GetEventManagementContext();
            UserDTO user = ArrangeHelper.ArrangeUser(context);

            EventDTO newEvent = new()
            {
                Id = Guid.NewGuid(),
                Title = Lorem.Sentence(),
                Description = Lorem.Paragraph(),
                Location = Lorem.Sentence(),
                Date = DateTime.Now,
                Duration = TimeSpan.FromHours(1),
                OwnerId = user.Id,
                Status = 0
            };

            // Act

            string result = await _repositoryManager.Event.CreateEvent(newEvent, default);
            await _repositoryManager.SaveAsync(default);

            // Assert
            using EventManagementContext newContext = TestContext.GetEventManagementContext();
            bool eventExist = await newContext.Events.AnyAsync(t => t.Id == newEvent.Id, default);
            EventDTO eventFromDb = await newContext.Events.FirstOrDefaultAsync(t => t.Id == newEvent.Id, default);
            ArrangeHelper.CleanUp(context, user, newEvent);
            eventExist.ShouldBeTrue();
            result.ShouldBe(newEvent.Id.ToString());
            _ = eventFromDb.ShouldNotBeNull();
            eventFromDb.Id.ShouldBe(newEvent.Id);
            eventFromDb.Title.ShouldBe(newEvent.Title);
            eventFromDb.Description.ShouldBe(newEvent.Description);
            eventFromDb.Location.ShouldBe(newEvent.Location);
            eventFromDb.Date.ShouldBe(CutLastMillisecond(newEvent.Date));
            eventFromDb.Duration.ShouldBe(newEvent.Duration);
            eventFromDb.OwnerId.ShouldBe(newEvent.OwnerId);
            eventFromDb.Status.ShouldBe(newEvent.Status);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ChangeEventStatusShouldChangeEventStatus(int status)
        {
            // Arrange
            using EventManagementContext context = TestContext.GetEventManagementContext();
            UserDTO user = ArrangeHelper.ArrangeUser(context);
            EventDTO eventToChange = ArrangeHelper.ArrangeEvent(context, user);

            // Act
            await _repositoryManager.Event.ChangeEventStatus(eventToChange.Id.ToString(), (EventStatus)status, default);
            await _repositoryManager.SaveAsync(default);

            // Assert
            using EventManagementContext newContext = TestContext.GetEventManagementContext();
            EventDTO eventFromDb = await newContext.Events.FirstOrDefaultAsync(t => t.Id == eventToChange.Id, default);
            ArrangeHelper.CleanUp(newContext, user, eventToChange);
            eventFromDb.Status.ShouldBe(status);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task ChangeEventStatusShouldThrowException(int status)
        {
            // Arrange
            using EventManagementContext context = TestContext.GetEventManagementContext();
            UserDTO userDTO = ArrangeHelper.ArrangeUser(context);
            EventDTO eventToChange = new()
            {
                Id = Guid.NewGuid(),
                Title = Lorem.Sentence(),
                Description = Lorem.Paragraph(),
                Location = Lorem.Sentence(),
                Date = DateTime.Now,
                Duration = TimeSpan.FromHours(1),
                OwnerId = userDTO.Id,
                Status = status
            };
            _ = await context.Events.AddAsync(eventToChange, default);
            _ = await context.SaveChangesAsync(default);

            // Act
            Func<Task> action = async () => await _repositoryManager.Event.ChangeEventStatus(eventToChange.Id.ToString(), (EventStatus)status, default);

            // Assert
            if (status == 0)
            {
                action.ShouldThrow<EventStatusException>().Error.ShouldBe(EventStatusError.NewStatusCannotBeSameAsOldStatus);
            }
            else
            {
                action.ShouldThrow<EventStatusException>().Error.ShouldBe(EventStatusError.StatusCannotBeChanged);
            }
            using EventManagementContext newContext = TestContext.GetEventManagementContext();
            ArrangeHelper.CleanUp(newContext, userDTO, eventToChange);
        }
        [Fact]
        public async Task DeleteEventByIdShouldDeleteEvent()
        {
            // Arrange
            using EventManagementContext context = TestContext.GetEventManagementContext();
            UserDTO userDTO = ArrangeHelper.ArrangeUser(context);
            EventDTO eventToDelete = ArrangeHelper.ArrangeEvent(context, userDTO);

            // Act
            _ = await _repositoryManager.Event.DeleteEventById(eventToDelete.Id.ToString(), default);
            await _repositoryManager.SaveAsync(default);

            // Assert
            using EventManagementContext newContext = TestContext.GetEventManagementContext();
            bool eventExist = await newContext.Events.AnyAsync(t => t.Id == eventToDelete.Id, default);
            ArrangeHelper.CleanUp(newContext, userDTO);
            eventExist.ShouldBeFalse();
        }

        [Fact]
        public async Task GetEventByIdShouldReturnEvent()
        {
            // Arrange
            using EventManagementContext context = TestContext.GetEventManagementContext();
            UserDTO userDTO = ArrangeHelper.ArrangeUser(context);
            EventDTO eventToSearch = ArrangeHelper.ArrangeEvent(context, userDTO);
            // Act
            EventDTO result = await _repositoryManager.Event.GetEventById(eventToSearch.Id.ToString(), default);
            // Assert
            using EventManagementContext newContext = TestContext.GetEventManagementContext();
            ArrangeHelper.CleanUp(newContext, userDTO, eventToSearch);
            _ = result.ShouldNotBeNull();
            result.Id.ShouldBe(eventToSearch.Id);
            result.Title.ShouldBe(eventToSearch.Title);
            result.Description.ShouldBe(eventToSearch.Description);
            result.Location.ShouldBe(eventToSearch.Location);
            result.Date.ShouldBe(CutLastMillisecond(eventToSearch.Date));
            result.Duration.ShouldBe(eventToSearch.Duration);
            result.OwnerId.ShouldBe(eventToSearch.OwnerId);
            result.Status.ShouldBe(eventToSearch.Status);
        }
        private static DateTime CutLastMillisecond(DateTime dateTime)
        {
            return DateTime.Parse(dateTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff"));
        }
    }
}