using System;
using System.Threading.Tasks;

using Contracts.Models.Statuses;

using Domain.Commands;
using Domain.Interfaces;
using Domain.Models.Database;

using Moq;

using Shouldly;

namespace UnitTests.Commands
{
    public class CancelEventByIdCommandTest
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new();
        private readonly Mock<IEventRepository> _eventRepositoryMock = new();
        private readonly CancelEventByIdCommandHandler _cancelEventByIdCommandHandler;
        public CancelEventByIdCommandTest()
        {
            _ = _repositoryManagerMock.Setup(x => x.Event).Returns(_eventRepositoryMock.Object);
            _cancelEventByIdCommandHandler = new CancelEventByIdCommandHandler(_repositoryManagerMock.Object);
        }
        [Fact]
        public async Task CancelEventByIdCommandHandlerShouldCancelEvent()
        {
            // Arrange
            string id = Guid.NewGuid().ToString();
            _ = _repositoryManagerMock.Setup(x => x.Event.GetEventById(id, default)).ReturnsAsync(new EventDTO
            {
                Id = Guid.Parse(id),
                Status = 0
            });
            // Act
            CancelEventByIdResult result = await _cancelEventByIdCommandHandler.Handle(new CancelEventByIdCommand
            {
                Id = id
            }, default);
            // Assert
            _repositoryManagerMock.Verify(x => x.Event.ChangeEventStatus(id, EventStatus.Cancelled, default), Times.Once);
            _ = result.ShouldNotBeNull();
        }
    }
}