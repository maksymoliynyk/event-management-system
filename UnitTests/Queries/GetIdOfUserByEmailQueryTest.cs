using System;
using System.Threading.Tasks;

using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Queries;

using Faker;

using Moq;

using Shouldly;

namespace UnitTests.Queries
{
    public class GetIdOfUserByEmailQueryTest
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly GetIdOfUserByEmailQueryHandler _getIdOfUserByEmailQueryHandler;
        public GetIdOfUserByEmailQueryTest()
        {
            _ = _repositoryManagerMock.Setup(x => x.User).Returns(_userRepositoryMock.Object);
            _getIdOfUserByEmailQueryHandler = new GetIdOfUserByEmailQueryHandler(_repositoryManagerMock.Object);
        }
        [Fact]
        public async Task GetIdOfUserByEmailShouldReturnId()
        {
            // Arrange
            Guid eventId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            string userEmail = Internet.Email();
            _ = _userRepositoryMock.Setup(x => x.GetUserByEmailOrCreateUser(It.IsAny<string>(), default)).ReturnsAsync(new UserDTO
            {
                Id = userId,
                Email = userEmail
            });
            // Act
            GetIdOfUserByEmailResult result = await _getIdOfUserByEmailQueryHandler.Handle(new GetIdOfUserByEmailQuery
            {
                Email = userEmail
            }, default);
            // Assert
            _ = result.ShouldNotBeNull();
            _ = result.Id.ShouldNotBeNull();
            _repositoryManagerMock.Verify(x => x.User.GetUserByEmailOrCreateUser(It.IsAny<string>(), default), Times.Once);
        }

    }
}