using System.Threading;
using System.Threading.Tasks;

using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Queries.UserQueries;

using Moq;

namespace UnitTests.Queries.UserQueries
{
    public class GetInfoAboutUserQueryTests
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock;
        private readonly GetInfoAboutUserQueryHandler _handler;

        public GetInfoAboutUserQueryTests()
        {
            _repositoryManagerMock = new Mock<IRepositoryManager>();
            _handler = new GetInfoAboutUserQueryHandler(_repositoryManagerMock.Object);
        }

        [Fact]
        public async Task HandleValidGetInfoAboutUserQueryReturnsUserInfo()
        {
            // Arrange
            GetInfoAboutUserQuery query = new()
            {
                UserName = "user123"
            };

            UserDTO userDto = new()
            {
                UserName = "user123",
                Email = "john.doe@example.com"
            };

            _ = _repositoryManagerMock.Setup(r => r.User.GetUserByUsername(query.UserName, CancellationToken.None))
                                      .ReturnsAsync(userDto);

            // Act
            GetInfoAboutUserResult result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userDto.UserName, result.UserName);
            Assert.Equal(userDto.Email, result.Email);
        }
    }
}
