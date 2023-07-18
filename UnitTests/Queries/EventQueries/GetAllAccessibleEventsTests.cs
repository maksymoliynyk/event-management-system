using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Queries.EventQueries;

using Moq;

namespace UnitTests.Queries.EventQueries
{
    public class GetAllAccessibleEventsQueryTests
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllAccessibleEventsHandler _handler;

        public GetAllAccessibleEventsQueryTests()
        {
            _repositoryManagerMock = new Mock<IRepositoryManager>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetAllAccessibleEventsHandler(_repositoryManagerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task HandleValidGetAllAccessibleEventsQueryReturnsAllEvents()
        {
            // Arrange
            GetAllAccessibleEventsQuery query = new()
            {
                UserId = "user123"
            };

            IEnumerable<EventDTO> eventDtos = new List<EventDTO>
            {
                new EventDTO { Id = "event1", Title = "Event 1" },
                new EventDTO { Id = "event2", Title = "Event 2" },
                new EventDTO { Id = "event3", Title = "Event 3" }
            };

            IEnumerable<Event> mappedEvents = new List<Event>
            {
                new Event { Id = "event1", Title = "Event 1" },
                new Event { Id = "event2", Title = "Event 2" },
                new Event { Id = "event3", Title = "Event 3" }
            };

            _ = _repositoryManagerMock.Setup(r => r.Event.GetAllEvents(query.UserId, CancellationToken.None))
                                      .ReturnsAsync(eventDtos);

            _ = _mapperMock.Setup(m => m.Map<IEnumerable<Event>>(eventDtos))
                            .Returns(mappedEvents);

            // Act
            GetAllAccessibleEventsResult result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mappedEvents, result.Events);
        }
    }
}