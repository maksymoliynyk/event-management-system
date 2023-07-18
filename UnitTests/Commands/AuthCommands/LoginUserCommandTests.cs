using System.Threading;
using System.Threading.Tasks;

using Domain.Commands.AuthCommands;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;

using Moq;

namespace UnitTests.Commands.AuthCommands
{
    public class LoginUserCommandTests
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly LoginUserCommandHandler _handler;

        public LoginUserCommandTests()
        {
            _repositoryManagerMock = new Mock<IRepositoryManager>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _ = _repositoryManagerMock.Setup(r => r.User).Returns(_userRepositoryMock.Object);
            _handler = new LoginUserCommandHandler(_repositoryManagerMock.Object);
        }

        [Fact]
        public async Task HandleValidCredentialsReturnsLoginUserResultWithToken()
        {
            // Arrange
            string email = "test@example.com";
            string password = "password123";

            // Set up the repository manager mock to return a token
            TokenModel expectedToken = new() { };
            _ = _userRepositoryMock.Setup(u => u.LoginUser(email, password, It.IsAny<CancellationToken>()))
                              .ReturnsAsync(expectedToken);

            LoginUserCommand command = new() { Email = email, Password = password };

            // Act
            LoginUserResult result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedToken, result.Token);
        }

        [Fact]
        public async Task HandleRepositoryExceptionThrowsException()
        {
            // Arrange
            string email = "test@example.com";
            string password = "password123";

            // Set up the repository manager mock to throw an exception
            _ = _userRepositoryMock.Setup(u => u.LoginUser(email, password, It.IsAny<CancellationToken>()))
                              .ThrowsAsync(new LoginException(LoginExceptionError.PasswordIncorrect, "Password incorrect"));

            LoginUserCommand command = new() { Email = email, Password = password };

            // Act & Assert
            _ = await Assert.ThrowsAsync<LoginException>(async () => await _handler.Handle(command, CancellationToken.None));
        }
    }
}