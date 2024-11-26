using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Domain.Aggregates.Events;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Models.Database;

using Infrastructure;
using Infrastructure.Repositories;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

namespace IntegrationTests.Repository
{
    [Collection("Database")]
    public class EventRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<EventManagementContext> _dbContextOptions;
        private readonly EventManagementContext _dbContext;
        private readonly UserManager<UserDTO> _userManager;
        private readonly IEventRepository _eventRepository;
        private readonly RSVPDTO _testRSVP;
        private readonly UserDTO _testOwner;
        private readonly string _testOwnerPassword;
        private readonly UserDTO _testInvitee;
        private readonly string _testInviteePassword;
        private readonly EventDTO _testEvent;
        private readonly EventDTO _testPublicEvent;
        private readonly List<string> _testRegisterData;
        public EventRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<EventManagementContext>()
                .UseNpgsql("Host=localhost;Database=EventManagement;Username=postgres;Password=db_learn")
                .Options;

            _dbContext = CreateDbContext();
            _userManager = CreateUserManager();
            _eventRepository = new EventRepository(_dbContext);
            UserDTO testOwner = new()
            {
                Email = Internet.Email(),
                UserName = Internet.UserName(),
                FirstName = Name.First(),
                LastName = Name.Last()
            };
            _testOwnerPassword = Guid.NewGuid().ToString();
            UserDTO testInvitee = new()
            {
                Email = Internet.Email(),
                UserName = Internet.UserName(),
                FirstName = Name.First(),
                LastName = Name.Last()
            };
            _testInviteePassword = Guid.NewGuid().ToString();
            UserDTO userOwner = null;
            UserDTO userInvitee = null;
            Task.Run(async () =>
                    {
                        _ = await _userManager.CreateAsync(testOwner, _testOwnerPassword);
                        _ = await _userManager.CreateAsync(testInvitee, _testInviteePassword);
                        _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
                        userOwner = await _userManager.FindByEmailAsync(testOwner.Email);
                        userInvitee = await _userManager.FindByEmailAsync(testInvitee.Email);
                    }).GetAwaiter().GetResult();
            _testOwner = userOwner;
            _testInvitee = userInvitee;
            _testEvent = new EventDTO
            {
                Id = Guid.NewGuid().ToString(),
                Title = Company.Name(),
                Description = Lorem.Sentence(),
                OwnerId = _testOwner.Id,
                Location = Address.StreetAddress(),
                Date = GetCurrentDateTime(),
                Duration = TimeSpan.FromHours(2),
                Status = 0,
                IsPublic = false
            };
            _testPublicEvent = new EventDTO
            {
                Id = Guid.NewGuid().ToString(),
                Title = Company.Name(),
                Description = Lorem.Sentence(),
                OwnerId = _testOwner.Id,
                Location = Address.StreetAddress(),
                Date = GetCurrentDateTime(),
                Duration = TimeSpan.FromHours(2),
                Status = 0,
                IsPublic = true
            };
            _testRSVP = new RSVPDTO
            {
                Id = Guid.NewGuid().ToString(),
                EventId = _testEvent.Id,
                UserId = _testInvitee.Id,
                Status = 0
            };
            Task.Run(async () =>
            {
                _ = await _dbContext.Events.AddAsync(_testEvent, CancellationToken.None);
                _ = await _dbContext.RSPVs.AddAsync(_testRSVP, CancellationToken.None);
                _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
            }).GetAwaiter().GetResult();
            _testRegisterData = new List<string>
            {
                _testOwner.Id,
                _testInvitee.Id
            };
        }

        [Fact]
        public async Task ValidGetEventByIdShouldReturnEvent()
        {
            // Arrange
            // Act
            EventDTO result = await _eventRepository.GetEventById(_testEvent.Id, CancellationToken.None);
            // Assert
            Assert.Equal(_testEvent.Id, result.Id);
            Assert.Equal(_testEvent.Title, result.Title);
            Assert.Equal(_testEvent.Date, result.Date);
            Assert.Equal(_testEvent.Description, result.Description);
            Assert.Equal(_testEvent.Duration, result.Duration);
            Assert.Equal(_testEvent.OwnerId, result.OwnerId);
            Assert.Equal(_testEvent.Location, result.Location);
            Assert.Equal(_testEvent.IsPublic, result.IsPublic);
            Assert.Equal(_testEvent.Status, result.Status);
        }
        [Fact]
        public async Task InvalidGetEventByIdShouldThrowAnException()
        {
            // Arrange
            string nonExistedId = Guid.NewGuid().ToString();
            // Act & Assert
            ObjectNotFoundException exception = await Assert.ThrowsAnyAsync<ObjectNotFoundException>(async () => await _eventRepository.GetEventById(nonExistedId, CancellationToken.None));
            Assert.Equal(ObjectNotFoundErrors.Event, exception.Error);
        }
        [Fact]
        public async Task ValidGetEventsByOwnerShouldReturnIEnumarableEvents()
        {
            // Arrange
            string[] eventsId = _dbContext.Events.Where(x => x.OwnerId == _testOwner.Id)
                                            .Select(x => x.Id)
                                            .ToArray();
            // Act
            IEnumerable<EventDTO> result = await _eventRepository.GetEventsByOwner(_testOwner.Id, CancellationToken.None);
            // Assert
            Assert.NotEmpty(result);
            foreach (EventDTO item in result)
            {
                Assert.True(eventsId.FirstOrDefault(x => x == item.Id) != null);
            }
        }
        [Fact]
        public async Task ValidGetEventsByOwnerShouldReturnEmptyIEnumarableEventsIfRSVPsForThisEventDoesntExists()
        {
            // Arrange
            string nonExistedId = Guid.NewGuid().ToString();
            // Act
            IEnumerable<EventDTO> result = await _eventRepository.GetEventsByOwner(nonExistedId, CancellationToken.None);
            // Assert
            Assert.Empty(result);
        }
        [Fact]
        public async Task ValidGetAllEventsWhereUserAreParticipateShouldReturnIEnumarableEvent()
        {
            // Arrange
            string[] idRsvpEvents = _dbContext.RSPVs.Where(x => x.UserId == _testInvitee.Id)
                                                .Select(x => x.EventId)
                                                .ToArray();
            // Act
            IEnumerable<EventDTO> result = await _eventRepository.GetAllEventsWhereUserAreParticipate(_testInvitee.Id, CancellationToken.None);
            // Assert
            Assert.NotEmpty(result);
            foreach (EventDTO item in result)
            {
                Assert.Contains(item.Id, idRsvpEvents);
            }
        }
        [Fact]
        public async Task InvalidGetAllEventsWhereUserAreParticipateShouldReturnEmptyIEnumarableRSVP()
        {
            // Arrange
            string nonExistedId = Guid.NewGuid().ToString();
            // Act
            IEnumerable<EventDTO> result = await _eventRepository.GetAllEventsWhereUserAreParticipate(nonExistedId, CancellationToken.None);
            // Assert
            Assert.Empty(result);
        }
        [Fact]
        public async Task ValidIsUserOwnerShouldReturnTrue()
        {
            // Arrange
            // Act
            bool result = await _eventRepository.IsUserOwner(_testOwner.Id, _testEvent.Id, CancellationToken.None);
            // Assert
            Assert.True(result);
        }
        [Fact]
        public async Task InvalidIsUserOwnerShouldReturnFalse()
        {
            // Arrange
            string nonExistedId = Guid.NewGuid().ToString();
            // Act
            bool result = await _eventRepository.IsUserOwner(nonExistedId, _testEvent.Id, CancellationToken.None);
            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAllEventsShouldReturnOnlyPublicEventsIfUserIsNull()
        {
            // Arrange
            // Act
            IEnumerable<EventDTO> result = await _eventRepository.GetAllEvents(null, CancellationToken.None);
            // Assert
            Assert.NotEmpty(result);
            foreach (EventDTO item in result)
            {
                Assert.True(item.IsPublic);
            }
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task GetAllEventsShouldReturnIEnumarableEvents(int index)
        {
            // Arrange
            string userId = _testRegisterData[index];
            List<string> rsvpsIDs = _dbContext.RSPVs.Where(x => x.UserId == userId)
                                    .Select(x => x.EventId)
                                    .ToList();
            List<string> ids = _dbContext.Events.Include(x => x.Owner)
                                    .Where(x => rsvpsIDs.Contains(x.Id) || x.OwnerId == userId || x.IsPublic)
                                    .Select(x => x.Id)
                                    .ToList();
            // Act
            IEnumerable<EventDTO> result = await _eventRepository.GetAllEvents(userId, CancellationToken.None);
            // Assert
            Assert.NotEmpty(result);
            foreach (EventDTO item in result)
            {
                Assert.Contains(item.Id, ids);
            }
        }

        [Fact]
        public async Task CreateEventShouldCreateEvent()
        {
            // Arrange
            EventDTO newEvent = new()
            {
                Id = Guid.NewGuid().ToString(),
                Title = Company.Name(),
                Description = Lorem.Sentence(),
                OwnerId = _testOwner.Id,
                Location = Address.StreetAddress(),
                Date = GetCurrentDateTime(),
                Duration = TimeSpan.FromHours(2),
                Status = 0,
                IsPublic = false
            };
            // Act
            string resultId = await _eventRepository.CreateEvent(newEvent, CancellationToken.None);
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
            EventDTO result = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == resultId, CancellationToken.None);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(newEvent.Id, result.Id);
            Assert.Equal(newEvent.Title, result.Title);
            Assert.Equal(newEvent.Date, result.Date);
            Assert.Equal(newEvent.Description, result.Description);
            Assert.Equal(newEvent.Duration, result.Duration);
            Assert.Equal(newEvent.OwnerId, result.OwnerId);
            Assert.Equal(newEvent.Location, result.Location);
            Assert.Equal(newEvent.IsPublic, result.IsPublic);
            Assert.Equal(newEvent.Status, result.Status);

            _ = _dbContext.Events.Remove(newEvent);
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
        }
        [Fact]
        public async Task DeleteEventShouldDeleteEvent()
        {
            // Arrange
            // Act
            EventDTO result = await _eventRepository.DeleteEventById(_testEvent.Id);
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
            // Assert
            Assert.NotNull(result);
            Assert.Null(await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == _testEvent.Id, CancellationToken.None));

            _ = await _dbContext.Events.AddAsync(_testEvent, CancellationToken.None);
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
        }
        [Theory]
        [InlineData(EventStatus.Cancelled)]
        [InlineData(EventStatus.Finished)]
        public async Task ValidChangeStatusShouldChangeStatus(EventStatus status)
        {
            // Arrange
            // Act
            await _eventRepository.ChangeEventStatus(_testEvent.Id, status, CancellationToken.None);
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);

            EventDTO newEvent = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == _testEvent.Id);
            // Assert
            Assert.NotNull(newEvent);
            Assert.Equal((int)status, newEvent.Status);

            newEvent.Status = 0;
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
        }
        [Theory]
        [InlineData(EventStatus.InProgress, ObjectStatusError.NewStatusCannotBeSameAsOldStatus, ObjectType.Event)]
        [InlineData(EventStatus.Cancelled, ObjectStatusError.StatusCannotBeChanged, ObjectType.Event)]
        [InlineData(EventStatus.Finished, ObjectStatusError.StatusCannotBeChanged, ObjectType.Event)]
        public async Task InvalidChangeStatusShouldThrowExcception(EventStatus status, ObjectStatusError error, ObjectType type)
        {
            // Arrange
            EventDTO newEvent = await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == _testEvent.Id);
            newEvent.Status = (int)status;
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
            // Act
            ObjectStatusException exception = await Assert.ThrowsAnyAsync<ObjectStatusException>(async () => await _eventRepository.ChangeEventStatus(_testEvent.Id, status, CancellationToken.None));
            // Assert
            Assert.Equal(error, exception.Error);
            Assert.Equal(type, exception.Type);

            newEvent.Status = 0;
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
        }

        private EventManagementContext CreateDbContext()
        {
            EventManagementContext dbContext = new(_dbContextOptions);
            _ = dbContext.Database.EnsureCreated();
            return dbContext;
        }

        private UserManager<UserDTO> CreateUserManager()
        {
            UserStore<UserDTO> userStore = new(_dbContext);
            PasswordHasher<UserDTO> passwordHasher = new();
            Mock<ILogger<UserManager<UserDTO>>> mockLogger = new();

            UserManager<UserDTO> userManager = new(
                userStore,
                null,
                passwordHasher,
                null,
                null,
                null,
                null,
                null,
                mockLogger.Object
            );
            userManager.Options.User.RequireUniqueEmail = true;
            userManager.Options.Password.RequireDigit = false;
            userManager.Options.Password.RequiredLength = 6;
            userManager.Options.Password.RequireNonAlphanumeric = false;
            userManager.Options.Password.RequireUppercase = false;
            userManager.Options.Password.RequireLowercase = false;
            CustomUserValidator customUserValidator = new(userManager);
            CustomPasswordValidator customPasswordValidator = new();
            userManager.UserValidators.Add(customUserValidator);
            userManager.PasswordValidators.Add(customPasswordValidator);
            return userManager;
        }
        private static DateTime GetCurrentDateTime()
        {
            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime parsedDateTime = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            return parsedDateTime;
        }
        public async void Dispose()
        {
            _ = _dbContext.RSPVs.Remove(_testRSVP);
            _ = _dbContext.Events.Remove(_testEvent);
            _ = await _userManager.DeleteAsync(_testInvitee);
            _ = await _userManager.DeleteAsync(_testOwner);
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
            _dbContext.Dispose();
        }
    }
}