using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Queries.UserQueries;

using Moq;

namespace UnitTests.Queries.UserQueries
{
    public class GetAllEventsWhereUserAreParticipateQueryTests
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllEventsWhereUserAreParticipateHandler _handler;

        public GetAllEventsWhereUserAreParticipateQueryTests()
        {
            _repositoryManagerMock = new Mock<IRepositoryManager>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetAllEventsWhereUserAreParticipateHandler(_mapperMock.Object, _repositoryManagerMock.Object);
        }

        [Fact]
        public async Task HandleValidGetAllEventsWhereUserAreParticipateQueryReturnsAllEvents()
        {
            // Arrange
            GetAllEventsWhereUserAreParticipateQuery query = new()
            {
                UserId = "user123"
            };

            IEnumerable<EventDTO> eventDtos = new List<EventDTO>
            {
                new EventDTO { Id = "event1", Title = "Event 1", OwnerId = "user123" },
                new EventDTO { Id = "event2", Title = "Event 2", OwnerId = "user123" },
                new EventDTO { Id = "event3", Title = "Event 3", OwnerId = "user123" }
            };

            IEnumerable<Event> mappedEvents = new List<Event>
            {
                new Event { Id = "event1", Title = "Event 1", OwnerEmail = "user123" },
                new Event { Id = "event2", Title = "Event 2", OwnerEmail = "user123" },
                new Event { Id = "event3", Title = "Event 3", OwnerEmail = "user123" }
            };

            _ = _repositoryManagerMock.Setup(r => r.Event.GetAllEventsWhereUserAreParticipate(query.UserId, CancellationToken.None))
                                      .ReturnsAsync(eventDtos);

            _ = _mapperMock.Setup(m => m.Map<IEnumerable<Event>>(eventDtos))
                            .Returns(mappedEvents);

            // Act
            GetAllEventsWhereUserAreParticipateResult result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mappedEvents, result.Events);
        }
    }
}
