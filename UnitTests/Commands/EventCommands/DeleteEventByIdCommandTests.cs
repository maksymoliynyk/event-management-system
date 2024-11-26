using System.Threading;
using System.Threading.Tasks;

using Application.Commands.EventCommands;

using Domain.Aggregates.Events;
using Domain.Exceptions;

using Infrastructure;

using Moq;

namespace UnitTests.Commands.EventCommands
{
    public class DeleteEventByIdCommandTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly DeleteEventByIdCommandHandler _handler;
        public DeleteEventByIdCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _ = _unitOfWorkMock.Setup(r => r.Event).Returns(_eventRepositoryMock.Object);
            _handler = new DeleteEventByIdCommandHandler(_unitOfWorkMock.Object);
        }
        [Fact]
        public async Task HandleValidEventDeleteReturnsNotNullDeleteEventByIdResult()
        {
            // Arrange
            DeleteEventByIdCommand command = new()
            {
                Id = "event123",
                UserId = "user123"
            };

            _ = _unitOfWorkMock.Setup(r => r.Event.IsUserOwner(command.UserId, command.Id, CancellationToken.None))
                                  .ReturnsAsync(true);

            // Act
            DeleteEventByIdResult result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
        }
        [Fact]
        public async Task HandleInvalidEventDeleteThrowsExceptionWhenUserHasNoAccess()
        {
            // Arrange
            DeleteEventByIdCommand command = new()
            {
                Id = "event123",
                UserId = "user123"
            };

            _ = _unitOfWorkMock.Setup(r => r.Event.IsUserOwner(command.UserId, command.Id, CancellationToken.None))
                                  .ReturnsAsync(false);
            // Assert
            _ = await Assert.ThrowsAsync<NoAccessException>(async () => await _handler.Handle(command, CancellationToken.None));
        }
    }
}