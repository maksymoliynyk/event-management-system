using System;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Queries;

using Faker;

using Moq;

using Shouldly;

namespace UnitTests.Queries
{
    public class GetEventByIdQueryTest
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IEventRepository> _eventRepositoryMock = new();
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetEventByIdQueryHandler _getEventByIdQueryHandler;
        public GetEventByIdQueryTest()
        {
            _mapperMock = new Mock<IMapper>();
            _ = _repositoryManagerMock.Setup(x => x.User).Returns(_userRepositoryMock.Object);
            _ = _repositoryManagerMock.Setup(x => x.Event).Returns(_eventRepositoryMock.Object);
            _getEventByIdQueryHandler = new GetEventByIdQueryHandler(_repositoryManagerMock.Object, _mapperMock.Object);
        }
        [Fact]
        public async Task GetEventByIdQueryHandlerShouldReturnEvent()
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
            _ = _eventRepositoryMock.Setup(x => x.GetEventById(It.IsAny<string>(), default)).ReturnsAsync(new EventDTO
            {
                Id = eventId,
                OwnerId = userId
            });
            _ = _mapperMock.Setup(x => x.Map<Event>(It.IsAny<EventDTO>())).Returns(new Event
            {
                Id = eventId.ToString(),
                OwnerEmail = userEmail
            });
            // Act
            GetEventByIdResult result = await _getEventByIdQueryHandler.Handle(new GetEventByIdQuery
            {
                Id = eventId.ToString()
            }, default);
            // Assert
            _ = result.ShouldNotBeNull();
            _ = result.SearchedEvent.ShouldNotBeNull();
            _repositoryManagerMock.Verify(x => x.Event.GetEventById(It.IsAny<string>(), default), Times.Once);
        }

    }
}