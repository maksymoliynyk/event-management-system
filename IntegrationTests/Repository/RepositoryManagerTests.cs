using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using Domain.Aggregates.Events;
using Domain.Aggregates.Users;
using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Services;

using Infrastructure;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

namespace IntegrationTests.Repository
{
    public class RepositoryManagerTests : IDisposable
    {
        private readonly DbContextOptions<EventManagementContext> _dbContextOptions;
        private readonly EventManagementContext _dbContext;
        private readonly UserManager<UserDTO> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserDTO _testUser;
        private readonly string _userPassword;
        private readonly Mock<TokenService> _tokenServiceMock;
        public RepositoryManagerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<EventManagementContext>()
                .UseNpgsql("Host=localhost;Database=EventManagement;Username=postgres;Password=db_learn")
                .Options;

            _dbContext = CreateDbContext();
            _userManager = CreateUserManager();
            _tokenServiceMock = new Mock<TokenService>();
            _unitOfWork = new UnitOfWork(_dbContext, _tokenServiceMock.Object, _userManager);
            _testUser = new UserDTO
            {
                Email = Internet.Email(),
                UserName = Internet.UserName(),
                FirstName = Name.First(),
                LastName = Name.Last()
            };
            _userPassword = Guid.NewGuid().ToString();
            Task.Run(async () =>
                    {
                        _ = await _userManager.CreateAsync(_testUser, _userPassword);
                        _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
                    }).GetAwaiter().GetResult();
        }
        [Fact]
        public async Task SaveAsyncShouldSave()
        {
            // Arrange
            EventDTO newEvent = new()
            {
                Id = Guid.NewGuid().ToString(),
                Title = Company.Name(),
                Description = Lorem.Sentence(),
                OwnerId = _testUser.Id,
                Location = Address.StreetAddress(),
                Date = GetCurrentDateTime(),
                Duration = TimeSpan.FromHours(2),
                Status = 0,
                IsPublic = false
            };
            _ = await _dbContext.Events.AddAsync(newEvent, CancellationToken.None);
            // Act
            await _unitOfWork.SaveAsync(CancellationToken.None);
            // Assert
            Assert.NotNull(await _dbContext.Events.FirstOrDefaultAsync(x => x.Id == newEvent.Id));
            _ = _dbContext.Events.Remove(newEvent);
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
        }
        [Fact]
        public void RSVPShouldReturnIRSVPRepositoryInstance()
        {
            // Arrange

            // Act
            IRSVPRepository rsvpRepository = _unitOfWork.RSVP;

            // Assert
            Assert.NotNull(rsvpRepository);
            _ = Assert.IsAssignableFrom<IRSVPRepository>(rsvpRepository);
        }
        [Fact]
        public void EventShouldReturnIEventRepositoryInstance()
        {
            // Arrange

            // Act
            IEventRepository eventRepository = _unitOfWork.Event;

            // Assert
            Assert.NotNull(eventRepository);
            _ = Assert.IsAssignableFrom<IEventRepository>(eventRepository);
        }
        [Fact]
        public void UserShouldReturnIRSVPRepositoryInstance()
        {
            // Arrange

            // Act
            IUserRepository userRepository = _unitOfWork.User;

            // Assert
            Assert.NotNull(userRepository);
            _ = Assert.IsAssignableFrom<IUserRepository>(userRepository);
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
            _ = await _userManager.DeleteAsync(_testUser);
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
            _dbContext.Dispose();
        }
        private static DateTime GetCurrentDateTime()
        {
            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime parsedDateTime = DateTime.ParseExact(formattedDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            return parsedDateTime;
        }
    }
}