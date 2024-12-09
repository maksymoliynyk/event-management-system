using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Domain.Aggregates.Events;

using Infrastructure;

using Moq;

namespace UnitTests.Queries.UserQueries
{
    public class GetAllEventsCreatedByUserQueryTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllEventsCreatedByUserQueryHandler _handler;

        public GetAllEventsCreatedByUserQueryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetAllEventsCreatedByUserQueryHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task HandleValidGetAllEventsCreatedByUserQueryReturnsAllEvents()
        {
            // Arrange
            GetAllEventsCreatedByUserQuery query = new()
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

            _ = _unitOfWorkMock.Setup(r => r.Event.GetEventsByOwner(query.UserId, CancellationToken.None))
                                      .ReturnsAsync(eventDtos);

            _ = _mapperMock.Setup(m => m.Map<IEnumerable<Event>>(eventDtos))
                            .Returns(mappedEvents);

            // Act
            GetAllEventsCreatedByUserResult result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mappedEvents, result.Events);
        }
    }
}
