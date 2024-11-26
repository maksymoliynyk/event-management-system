using System;
using System.Threading.Tasks;
using System.Threading;
using Domain.Models.Database;
using Domain.Services;
using Domain.Exceptions;
using Domain.Models;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Infrastructure;
using Infrastructure.Repositories;
using Domain.Aggregates.Users;

namespace IntegrationTests.Repository
{
    [Collection("Database")]
    public class UserRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<EventManagementContext> _dbContextOptions;
        private readonly EventManagementContext _dbContext;
        private readonly UserManager<UserDTO> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly UserDTO _testUser;
        private readonly string _userPassword;
        private readonly Mock<TokenService> _tokenServiceMock;
        private readonly List<object[]> _testRegisterData;
        public UserRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<EventManagementContext>()
                .UseNpgsql("Host=localhost;Database=EventManagement;Username=postgres;Password=db_learn")
                .Options;

            _dbContext = CreateDbContext();
            _userManager = CreateUserManager();
            _tokenServiceMock = new Mock<TokenService>();
            _userRepository = new UserRepository(_tokenServiceMock.Object, _userManager);
            _testUser = new UserDTO
            {
                Email = Internet.Email(),
                UserName = Internet.UserName(),
                FirstName = Name.First(),
                LastName = Name.Last()
            };
            _userPassword = Guid.NewGuid().ToString();
            _testRegisterData = new List<object[]>
            {
                new object[] { _testUser.Email, "ValidUserName", "ValidPassword" },
                new object[] { "newuser@example.com", _testUser.UserName, "ValidPassword" },
                new object[] { "newuser@example.com", "ValidUserName", "11111" }
            };
            Task.Run(async () =>
                    {
                        _ = await _userManager.CreateAsync(_testUser, _userPassword);
                        _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
                    }).GetAwaiter().GetResult();
        }

        [Fact]
        public async Task ValidGetUserByEmailShouldCreateUser()
        {
            // Arrange

            // Act
            UserDTO resultUser = await _userRepository.GetUserByEmail(_testUser.Email, CancellationToken.None);

            // Assert
            Assert.Equal(_testUser.Email, resultUser.Email);
            Assert.Equal(_testUser.UserName, resultUser.UserName);
            Assert.Equal(_testUser.FirstName, resultUser.FirstName);
            Assert.Equal(_testUser.LastName, resultUser.LastName);
        }
        [Fact]
        public async Task InvalidGetUserByEmailShouldThrowObjectNotFoundException()
        {
            // Arrange
            string fakeEmail = String.Empty;
            do
            {
                fakeEmail = Internet.Email();
            } while (fakeEmail == _testUser.Email);
            // Act & Assert
            ObjectNotFoundException exception = await Assert.ThrowsAnyAsync<ObjectNotFoundException>(async () => await _userRepository.GetUserByEmail(fakeEmail, CancellationToken.None));
            Assert.Equal(ObjectNotFoundErrors.User, exception.Error);
        }
        [Fact]
        public async Task ValidGetUserByUsernameShouldCreateUser()
        {
            // Arrange

            // Act
            UserDTO resultUser = await _userRepository.GetUserByUsername(_testUser.UserName, CancellationToken.None);

            // Assert
            Assert.Equal(_testUser.Email, resultUser.Email);
            Assert.Equal(_testUser.UserName, resultUser.UserName);
            Assert.Equal(_testUser.FirstName, resultUser.FirstName);
            Assert.Equal(_testUser.LastName, resultUser.LastName);
        }
        [Fact]
        public async Task InvalidGetUserByUsernameShouldThrowObjectNotFoundException()
        {
            // Arrange
            string fakeUserName = String.Empty;
            do
            {
                fakeUserName = Internet.UserName();
            } while (fakeUserName == _testUser.UserName);
            // Act & Assert
            ObjectNotFoundException exception = await Assert.ThrowsAnyAsync<ObjectNotFoundException>(async () => await _userRepository.GetUserByUsername(fakeUserName, CancellationToken.None));
            Assert.Equal(ObjectNotFoundErrors.User, exception.Error);
        }

        [Fact]
        public async Task ValidLoginUserShouldReturnTokenModel()
        {
            // Arrange
            TokenModel mockToken = new()
            {
                ExpiresIn = DateTime.Now,
                Token = Guid.NewGuid().ToString()
            };
            _ = _tokenServiceMock.Setup(x => x.CreateToken(It.IsAny<UserDTO>())).Returns(mockToken);
            // Act
            TokenModel result = await _userRepository.LoginUser(_testUser.Email, _userPassword, CancellationToken.None);
            // Assert
            Assert.Equal(mockToken.ExpiresIn, result.ExpiresIn);
            Assert.Equal(mockToken.Token, result.Token);
        }
        [Fact]
        public async Task InvalidLoginUserShouldThrowObjectNotFoundException()
        {
            // Arrange

            // Act & Assert
            LoginException exception = await Assert.ThrowsAnyAsync<LoginException>(async () => await _userRepository.LoginUser(_testUser.Email, "GetFirstWord", CancellationToken.None));
            Assert.Equal(LoginExceptionError.PasswordIncorrect, exception.Error);
        }
        [Fact]
        public async Task ValidRegisterUserShouldReturnSuccesfullIdentityResult()
        {
            // Arrange
            UserDTO newUser = new()
            {
                Email = Internet.Email(),
                UserName = Internet.UserName(),
                FirstName = Name.First(),
                LastName = Name.Last()
            };
            string newPassword = Guid.NewGuid().ToString();

            // Act
            IdentityResult result = await _userRepository.RegisterUser(newUser, newPassword);

            // Assert
            Assert.True(result.Succeeded);
            Assert.True(await _userManager.CheckPasswordAsync(newUser, newPassword));
            Assert.Empty(result.Errors);

            _ = await _userManager.DeleteAsync(newUser);
            _ = await _dbContext.SaveChangesAsync(CancellationToken.None);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task InvalidRegisterUserShouldReturnUnsuccesfullIdentityResultAndNotEmptyErrorList(int index)
        {
            // Arrange
            object[] testData = _testRegisterData[index];
            string email = (string)testData[0];
            string userName = (string)testData[1];
            string password = (string)testData[2];
            UserDTO newUser = new()
            {
                Email = email,
                UserName = userName,
                FirstName = Name.First(),
                LastName = Name.Last()
            };

            // Act
            IdentityResult result = await _userRepository.RegisterUser(newUser, password);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.Errors);
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
    }
}