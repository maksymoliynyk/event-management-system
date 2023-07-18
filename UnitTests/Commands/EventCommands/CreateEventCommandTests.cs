using System;
using System.Threading;
using System.Threading.Tasks;

using Domain.Commands.EventCommands;
using Domain.Interfaces;
using Domain.Models.Database;

using Moq;

namespace UnitTests.Commands.EventCommands
{
    public class CreateEventCommandTests
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly CreateEventCommandHandler _handler;
        public CreateEventCommandTests()
        {
            _repositoryManagerMock = new Mock<IRepositoryManager>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _ = _repositoryManagerMock.Setup(r => r.Event).Returns(_eventRepositoryMock.Object);
            _handler = new CreateEventCommandHandler(_repositoryManagerMock.Object);
        }
        [Fact]
        public async Task HandleValidEventCreationReturnsCreateEventResultWithId()
        {
            // Arrange
            CreateEventCommand command = new()
            {
                Title = "Test Event",
                Description = "Test event description",
                Date = DateTime.Now,
                Duration = 3600, // 1 hour in seconds
                Location = "Test Location",
                IsPublic = true,
                UserId = "user123"
            };

            EventDTO newEvent = new()
            {
                Id = "event123",
                Title = command.Title,
                Description = command.Description,
                Date = command.Date,
                Duration = TimeSpan.FromSeconds(command.Duration),
                Location = command.Location,
                OwnerId = command.UserId,
                Status = 0,
                IsPublic = command.IsPublic
            };

            _ = _eventRepositoryMock.Setup(r => r.CreateEvent(It.IsAny<EventDTO>(), CancellationToken.None))
                            .ReturnsAsync(newEvent.Id);

            // Act
            CreateEventResult result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newEvent.Id, result.Id);
        }
    }
}