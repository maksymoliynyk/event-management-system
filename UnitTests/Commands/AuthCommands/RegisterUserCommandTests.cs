using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Domain.Commands.AuthCommands;
using Domain.Interfaces;
using Domain.Models.Database;

using Microsoft.AspNetCore.Identity;

using Moq;

namespace UnitTests.Commands.AuthCommands
{
    public class RegisterUserCommandTests
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly RegisterUserCommandHandler _handler;
        private readonly Mock<IMapper> _mapperMock;

        public RegisterUserCommandTests()
        {
            _repositoryManagerMock = new Mock<IRepositoryManager>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _ = _repositoryManagerMock.Setup(r => r.User).Returns(_userRepositoryMock.Object);
            _handler = new RegisterUserCommandHandler(_repositoryManagerMock.Object, _mapperMock.Object);
        }
        [Fact]
        public async Task HandleValidRegistrationReturnsSuccessResult()
        {
            // Arrange
            RegisterUserCommand command = new()
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Password = "password123"
            };

            UserDTO userDTO = new() { /* Set up user DTO data */ };
            _ = _mapperMock.Setup(m => m.Map<RegisterUserCommand, UserDTO>(command))
                       .Returns(userDTO);

            IdentityResult identityResult = IdentityResult.Success;
            _ = _userRepositoryMock.Setup(u => u.RegisterUser(userDTO, command.Password, CancellationToken.None))
                               .ReturnsAsync(identityResult);

            // Act
            RegisterUserResult result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task HandleInvalidRegistrationReturnsFailureResultWithErrors()
        {
            // Arrange
            RegisterUserCommand command = new()
            {
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Password = "password123"
            };

            UserDTO userDTO = new() { /* Set up user DTO data */ };
            _ = _mapperMock.Setup(m => m.Map<RegisterUserCommand, UserDTO>(command))
                       .Returns(userDTO);

            IdentityError[] errors = new IdentityError[] { new IdentityError { /* Set up error details */ } };
            IdentityResult identityResult = IdentityResult.Failed(errors);
            _ = _userRepositoryMock.Setup(u => u.RegisterUser(userDTO, command.Password, CancellationToken.None))
                               .ReturnsAsync(identityResult);

            // Act
            RegisterUserResult result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(errors, result.Errors);
        }
    }
}