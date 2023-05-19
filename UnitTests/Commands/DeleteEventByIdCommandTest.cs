using System;
using System.Threading.Tasks;

using Domain.Commands;
using Domain.Interfaces;
using Domain.Models.Database;

using Moq;

using Shouldly;

namespace UnitTests.Commands
{
    public class DeleteEventByIdCommandTest
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new();
        private readonly Mock<IEventRepository> _eventRepositoryMock = new();
        private readonly DeleteEventByIdCommandHandler _deleteEventByIdCommandHandler;
        public DeleteEventByIdCommandTest()
        {
            _ = _repositoryManagerMock.Setup(x => x.Event).Returns(_eventRepositoryMock.Object);
            _deleteEventByIdCommandHandler = new DeleteEventByIdCommandHandler(_repositoryManagerMock.Object);
        }
        [Fact]
        public async Task DeleteEventByIdShouldDeleteEvent()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _ = _repositoryManagerMock.Setup(x => x.Event.DeleteEventById(id.ToString(), default)).ReturnsAsync(
               new EventDTO
               {
                   Id = id
               });
            // Act
            DeleteEventByIdResult result = await _deleteEventByIdCommandHandler.Handle(new DeleteEventByIdCommand
            {
                Id = id.ToString()
            }, default);
            // Assert
            _repositoryManagerMock.Verify(x => x.Event.DeleteEventById(id.ToString(), default), Times.Once);
            _ = result.ShouldNotBeNull();

        }
    }
}