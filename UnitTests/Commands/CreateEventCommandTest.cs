using System.Threading.Tasks;

using Domain.Interfaces;
using Domain.Commands;
using Domain.Models.Database;

using Moq;

using Faker;

using System;

using Shouldly;

namespace UnitTests.Commands
{
    public class CreateEventCommandTest
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new();
        private readonly Mock<IEventRepository> _eventRepositoryMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly CreateEventCommandHandler _createEventCommandHandler;

        public CreateEventCommandTest()
        {
            _ = _repositoryManagerMock.Setup(x => x.Event).Returns(_eventRepositoryMock.Object);
            _ = _repositoryManagerMock.Setup(x => x.User).Returns(_userRepositoryMock.Object);
            _createEventCommandHandler = new CreateEventCommandHandler(_repositoryManagerMock.Object);
        }
        [Fact]
        public async Task CreateEventCommandHandlerShouldReturnCreateEventResult()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            string title = Lorem.Sentence();
            string description = Lorem.Sentence();
            DateTime date = DateTime.Now;
            int duration = RandomNumber.Next();
            string location = Lorem.Sentence();
            string ownerEmail = Internet.Email();

            Guid userId = Guid.NewGuid();
            _ = _repositoryManagerMock.Setup(x => x.User.GetUserByEmailOrCreateUser(ownerEmail, default)).ReturnsAsync(new UserDTO
            {
                Id = userId,
                Email = ownerEmail
            });
            _ = _repositoryManagerMock.Setup(x => x.Event.CreateEvent(It.IsAny<EventDTO>(), default)).ReturnsAsync(id.ToString());
            // Act
            CreateEventResult result = await _createEventCommandHandler.Handle(new CreateEventCommand
            {
                Title = title,
                Description = description,
                Date = date,
                Duration = duration,
                Location = location,
                OwnerEmail = ownerEmail
            });

            // Assert
            _ = result.ShouldNotBeNull();
            result.Id.ShouldBe(id.ToString());
        }

    }
}