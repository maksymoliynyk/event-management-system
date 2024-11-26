using System.Threading;
using System.Threading.Tasks;

using Application.Queries.UserQueries;

using Domain.Models.Database;

using Infrastructure;

using Moq;

namespace UnitTests.Queries.UserQueries
{
    public class GetInfoAboutUserQueryTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly GetInfoAboutUserQueryHandler _handler;

        public GetInfoAboutUserQueryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new GetInfoAboutUserQueryHandler(_unitOfWorkMock.Object);
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

            _ = _unitOfWorkMock.Setup(r => r.User.GetUserByUsername(query.UserName, CancellationToken.None))
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
