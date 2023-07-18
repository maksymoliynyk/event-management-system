using System.Threading;
using System.Threading.Tasks;

using Domain.Commands.EventCommands;
using Domain.Exceptions;
using Domain.Interfaces;

using Moq;

namespace UnitTests.Commands.EventCommands
{
    public class CancelEventByIdCommandTests
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly CancelEventByIdCommandHandler _handler;
        public CancelEventByIdCommandTests()
        {
            _repositoryManagerMock = new Mock<IRepositoryManager>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _ = _repositoryManagerMock.Setup(r => r.Event).Returns(_eventRepositoryMock.Object);
            _handler = new CancelEventByIdCommandHandler(_repositoryManagerMock.Object);
        }
        [Fact]
        public async Task HandleValidEventCancellationReturnsNotNullCancelEventByIdResult()
        {
            // Arrange
            CancelEventByIdCommand command = new()
            {
                Id = "event123",
                UserId = "user123"
            };

            _ = _repositoryManagerMock.Setup(r => r.Event.IsUserOwner(command.UserId, command.Id, CancellationToken.None))
                                  .ReturnsAsync(true);

            // Act
            CancelEventByIdResult result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
        }
        [Fact]
        public async Task HandleInvalidEventCancellationThrowsExceptionWhenUserHasNoAccess()
        {
            // Arrange
            CancelEventByIdCommand command = new()
            {
                Id = "event123",
                UserId = "user123"
            };

            _ = _repositoryManagerMock.Setup(r => r.Event.IsUserOwner(command.UserId, command.Id, CancellationToken.None))
                                  .ReturnsAsync(false);
            // Assert
            _ = await Assert.ThrowsAsync<NoAccessException>(async () => await _handler.Handle(command, CancellationToken.None));
        }
    }
}