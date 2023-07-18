using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Queries.EventQueries;

using Moq;

namespace UnitTests.Queries.EventQueries
{
    public class GetEventByIdQueryTests
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetEventByIdQueryHandler _handler;

        public GetEventByIdQueryTests()
        {
            _repositoryManagerMock = new Mock<IRepositoryManager>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetEventByIdQueryHandler(_repositoryManagerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task HandlePublicEventReturnsGetEventByIdResultWithSearchedEvent()
        {
            // Arrange
            GetEventByIdQuery query = new()
            {
                Id = "event123",
                UserId = null
            };

            EventDTO searchedEventDto = new()
            {
                Id = "event123",
                Title = "Public Event",
                IsPublic = true
            };

            Event searchedEvent = new()
            {
                Id = "event123",
                Title = "Public Event"
            };

            _ = _repositoryManagerMock.Setup(r => r.Event.GetEventById(query.Id, CancellationToken.None))
                                      .ReturnsAsync(searchedEventDto);

            _ = _mapperMock.Setup(m => m.Map<Event>(searchedEventDto))
                            .Returns(searchedEvent);

            // Act
            GetEventByIdResult result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(searchedEvent, result.SearchedEvent);
        }

        [Fact]
        public async Task HandlePrivateEventWithAccessReturnsGetEventByIdResultWithSearchedEvent()
        {
            // Arrange
            GetEventByIdQuery query = new()
            {
                Id = "event123",
                UserId = "user123"
            };

            EventDTO searchedEventDto = new()
            {
                Id = "event123",
                Title = "Private Event",
                IsPublic = false,
                OwnerId = "user123"
            };

            Event searchedEvent = new()
            {
                Id = "event123",
                Title = "Private Event"
            };

            _ = _repositoryManagerMock.Setup(r => r.Event.GetEventById(query.Id, CancellationToken.None))
                                      .ReturnsAsync(searchedEventDto);

            _ = _repositoryManagerMock.Setup(r => r.RSVP.IsUserInvited(query.UserId, query.Id, CancellationToken.None))
                                      .ReturnsAsync(true);

            _ = _mapperMock.Setup(m => m.Map<Event>(searchedEventDto))
                            .Returns(searchedEvent);

            // Act
            GetEventByIdResult result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(searchedEvent, result.SearchedEvent);
        }

        [Fact]
        public async Task HandlePrivateEventWithoutAccessThrowsNoAccessException()
        {
            // Arrange
            GetEventByIdQuery query = new()
            {
                Id = "event123",
                UserId = "user123"
            };

            EventDTO searchedEventDto = new()
            {
                Id = "event123",
                Title = "Private Event",
                IsPublic = false,
                OwnerId = "otherUser456"
            };

            _ = _repositoryManagerMock.Setup(r => r.Event.GetEventById(query.Id, CancellationToken.None))
                                      .ReturnsAsync(searchedEventDto);

            _ = _repositoryManagerMock.Setup(r => r.RSVP.IsUserInvited(query.UserId, query.Id, CancellationToken.None))
                                      .ReturnsAsync(false);

            // Assert
            _ = await Assert.ThrowsAsync<NoAccessException>(async () => await _handler.Handle(query, CancellationToken.None));
        }
        [Fact]
        public async Task HandlePrivateEventWithoutUserIdThrowsNoAccessException()
        {
            // Arrange
            GetEventByIdQuery query = new()
            {
                Id = "event123",
                UserId = null
            };

            EventDTO searchedEventDto = new()
            {
                Id = "event123",
                Title = "Private Event",
                IsPublic = false,
                OwnerId = "otherUser456"
            };

            _ = _repositoryManagerMock.Setup(r => r.Event.GetEventById(query.Id, CancellationToken.None))
                                      .ReturnsAsync(searchedEventDto);

            _ = _repositoryManagerMock.Setup(r => r.RSVP.IsUserInvited(query.UserId, query.Id, CancellationToken.None))
                                      .ReturnsAsync(false);

            // Assert
            _ = await Assert.ThrowsAsync<NoAccessException>(async () => await _handler.Handle(query, CancellationToken.None));
        }
    }
}