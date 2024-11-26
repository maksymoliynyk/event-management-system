using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Domain.Aggregates.Events;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
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
    public class RSVPRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<EventManagementContext> _dbContextOptions;
        private readonly EventManagementContext _dbContext;
        private readonly UserManager<UserDTO> _userManager;
        private readonly IRSVPRepository _rsvpRepository;
        private readonly RSVPDTO _testRSVP;
        private readonly UserDTO _testOwner;
        private readonly string _testOwnerPassword;
        private readonly UserDTO _testInvitee;
        private readonly string _testInviteePassword;
        private readonly EventDTO _testEvent;
        public RSVPRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<EventManagementContext>()
                .UseNpgsql("Host=localhost;Database=EventManagement;Username=postgres;Password=db_learn")
                .Options;

            _dbContext = CreateDbContext();
            _userManager = CreateUserManager();
            _rsvpRepository = new RSVPRepository(_dbContext);
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
                Date = DateTime.Now,
                Duration = TimeSpan.FromHours(2),
                Status = 0,
                IsPublic = false
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
        }

        [Fact]
        public async Task ValidGetRSVPByIdShouldReturnRSVP()
        {
            // Arrange

            // Act
            RSVPDTO result = await _rsvpRepository.GetRSVPById(_testRSVP.Id, CancellationToken.None);
            // Assert
            Assert.Equal(_testRSVP.Id, result.Id);
            Assert.Equal(_testRSVP.EventId, result.EventId);
            Assert.Equal(_testRSVP.UserId, result.UserId);
            Assert.Equal(_testRSVP.Status, result.Status);
        }

        [Fact]
        public async Task InvalidGetRSVPByIdShouldThrowAnException()
        {
            // Arrange
            string wrongId = Guid.NewGuid().ToString();
            // Act & Assert
            ObjectNotFoundException exception = await Assert.ThrowsAnyAsync<ObjectNotFoundException>(async () => await _rsvpRepository.GetRSVPById(wrongId, CancellationToken.None));
            Assert.Equal(ObjectNotFoundErrors.RSVP, exception.Error);
        }

        [Fact]
        public async Task ValidGetAllRSVPsForEventShouldReturnIEnumarableEvents()
        {
            // Arrange
            // Act
            IEnumerable<RSVPDTO> result = await _rsvpRepository.GetAllRSVPsForEvent(_testEvent.Id, CancellationToken.None);
            // Assert
            Assert.NotEmpty(result);
        }
        [Fact]
        public async Task ValidGetAllRSVPsForEventShouldReturnEmptyIEnumarableEventsIfRSVPsForThisEventDoesntExists()
        {
            // Arrange
            string nonExistedId = Guid.NewGuid().ToString();
            // Act
            IEnumerable<RSVPDTO> result = await _rsvpRepository.GetAllRSVPsForEvent(nonExistedId, CancellationToken.None);
            // Assert
            Assert.Empty(result);
        }
        [Fact]
        public async Task ValidSendRSVPToUserShouldReturnRSVP()
        {
            // Arrange
            string username = Internet.UserName();
            string password = "111111";
            _ = await _userManager.CreateAsync(new()
            {
                Email = Internet.Email(),
                UserName = username,
                FirstName = Name.First(),
                LastName = Name.Last()
            }, password);
            UserDTO newUser = await _userManager.FindByNameAsync(username);
            // Act
            string resultId = (await _rsvpRepository.SendRSVPToUser(_testEvent.Id, newUser.Id, CancellationToken.None)).Id;
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
            RSVPDTO result = await _dbContext.RSPVs.FirstOrDefaultAsync(x => x.Id == resultId);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(newUser.Id, result.UserId);
            Assert.Equal(_testEvent.Id, result.EventId);
            Assert.Equal(0, result.Status);

            _ = _dbContext.RSPVs.Remove(result);
            _ = await _userManager.DeleteAsync(newUser);
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
        }

        [Fact]
        public async Task ValidGetAllRSVPsForUserShouldReturnIEnumarableRSVP()
        {
            // Arrange
            // Act
            IEnumerable<RSVPDTO> result = await _rsvpRepository.GetAllRSVPsForUser(_testInvitee.Id, CancellationToken.None);
            // Assert
            Assert.NotEmpty(result);
        }
        [Fact]
        public async Task InvalidGetAllRSVPsForUserShouldReturnEmptyIEnumarableRSVP()
        {
            // Arrange
            string nonExistedId = Guid.NewGuid().ToString();
            // Act
            IEnumerable<RSVPDTO> result = await _rsvpRepository.GetAllRSVPsForUser(nonExistedId, CancellationToken.None);
            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ValidIsUserInvitedShouldReturnTrue()
        {
            // Arrange
            // Act
            bool result = await _rsvpRepository.IsUserInvited(_testInvitee.Id, _testEvent.Id, CancellationToken.None);
            // Assert
            Assert.True(result);
        }
        [Fact]
        public async Task InvalidIsUserInvitedShouldReturnFalse()
        {
            // Arrange
            string nonExistedId = Guid.NewGuid().ToString();
            // Act
            bool result1 = await _rsvpRepository.IsUserInvited(nonExistedId, _testEvent.Id, CancellationToken.None);
            bool result2 = await _rsvpRepository.IsUserInvited(_testInvitee.Id, nonExistedId, CancellationToken.None);
            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }
        [Theory]
        [InlineData(RSVPStatus.Accepted)]
        [InlineData(RSVPStatus.Declined)]
        public async Task ValidChangeStatusShouldChangeStatus(RSVPStatus status)
        {
            // Arrange
            // Act
            await _rsvpRepository.ChangeRSVPStatus(_testRSVP.Id, status, CancellationToken.None);
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
            RSVPDTO newRSVP = await _dbContext.RSPVs.FirstOrDefaultAsync(x => x.Id == _testRSVP.Id);
            // Assert
            Assert.NotNull(newRSVP);
            Assert.Equal((int)status, newRSVP.Status);

            newRSVP.Status = 0;
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
        }
        [Theory]
        [InlineData(RSVPStatus.Pending, ObjectStatusError.NewStatusCannotBeSameAsOldStatus, ObjectType.RSVP)]
        [InlineData(RSVPStatus.Accepted, ObjectStatusError.StatusCannotBeChanged, ObjectType.RSVP)]
        [InlineData(RSVPStatus.Declined, ObjectStatusError.StatusCannotBeChanged, ObjectType.RSVP)]
        public async Task InvalidChangeStatusShouldThrowExcception(RSVPStatus status, ObjectStatusError error, ObjectType type)
        {
            // Arrange
            RSVPDTO newRSVP = (await _dbContext.RSPVs.AddAsync(new RSVPDTO
            {
                Id = Guid.NewGuid().ToString(),
                EventId = _testEvent.Id,
                UserId = _testOwner.Id,
                Status = (int)status
            }, CancellationToken.None)).Entity;
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
            // Act & Assert
            ObjectStatusException exception = await Assert.ThrowsAnyAsync<ObjectStatusException>(async () => await _rsvpRepository.ChangeRSVPStatus(newRSVP.Id, status, CancellationToken.None));
            Assert.Equal(error, exception.Error);
            Assert.Equal(type, exception.Type);

            _ = _dbContext.RSPVs.Remove(newRSVP);
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