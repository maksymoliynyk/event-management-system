using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Application.Queries.Events;

using AutoMapper;

using Contracts.Models;

using Domain.Aggregates.Events;

using Infrastructure;

using Moq;

namespace UnitTests.Queries.EventQueries
{
    public class GetAllEventsByUserQueryTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllEventsByUserQueryHandler _byUserQueryHandler;

        public GetAllEventsByUserQueryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _byUserQueryHandler = new GetAllEventsByUserQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task HandleValidGetAllAccessibleEventsQueryReturnsAllEvents()
        {
            // Arrange
            GetAllEventsByUserQuery byUserQuery = new()
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

            _ = _unitOfWorkMock.Setup(r => r.Event.GetAllEvents(byUserQuery.UserId, CancellationToken.None))
                                      .ReturnsAsync(eventDtos);

            _ = _mapperMock.Setup(m => m.Map<IEnumerable<Event>>(eventDtos))
                            .Returns(mappedEvents);

            // Act
            GetAllEventsByUserQueryResult byUserQueryResult = await _byUserQueryHandler.Handle(byUserQuery, CancellationToken.None);

            // Assert
            Assert.NotNull(byUserQueryResult);
            Assert.Equal(mappedEvents, byUserQueryResult.Events);
        }
    }
}