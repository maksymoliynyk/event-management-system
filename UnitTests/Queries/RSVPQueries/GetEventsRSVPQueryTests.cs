using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Queries.RSVPQueries;

using Moq;

namespace UnitTests.Queries.RSVPQueries
{
    public class GetEventsRSVPQueryTests
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetEventsRSVPQueryHandler _handler;

        public GetEventsRSVPQueryTests()
        {
            _repositoryManagerMock = new Mock<IRepositoryManager>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetEventsRSVPQueryHandler(_repositoryManagerMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task HandleValidGetEventsRSVPQueryReturnsAllRSVPs()
        {
            // Arrange
            GetEventsRSVPQuery query = new()
            {
                EventId = "event123",
                UserId = "user123"
            };

            IEnumerable<RSVPDTO> rsvpDtos = new List<RSVPDTO>
            {
                new RSVPDTO { Id = "rsvp1", UserId = "user1", EventId = "event123" },
                new RSVPDTO { Id = "rsvp2", UserId = "user2", EventId = "event123" },
                new RSVPDTO { Id = "rsvp3", UserId = "user3", EventId = "event123" }
            };

            IEnumerable<RSVP> mappedRSVPs = new List<RSVP>
            {
                new RSVP { Id = "rsvp1", UserEmail = "user1", EventTitle = "event123" },
                new RSVP { Id = "rsvp2", UserEmail = "user2", EventTitle = "event123" },
                new RSVP { Id = "rsvp3", UserEmail = "user3", EventTitle = "event123" }
            };

            _ = _repositoryManagerMock.Setup(r => r.Event.IsUserOwner(query.UserId, query.EventId, CancellationToken.None))
                                      .ReturnsAsync(true);

            _ = _repositoryManagerMock.Setup(r => r.RSVP.GetAllRSVPsForEvent(query.EventId, CancellationToken.None))
                                      .ReturnsAsync(rsvpDtos);

            _ = _mapperMock.Setup(m => m.Map<IEnumerable<RSVP>>(rsvpDtos))
                            .Returns(mappedRSVPs);

            // Act
            GetEventsRSVPResult result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mappedRSVPs, result.RSVPs);
        }

        [Fact]
        public async Task HandleInvalidGetEventsRSVPQueryThrowsNoAccessException()
        {
            // Arrange
            GetEventsRSVPQuery query = new()
            {
                EventId = "event123",
                UserId = "user123"
            };

            _ = _repositoryManagerMock.Setup(r => r.Event.IsUserOwner(query.UserId, query.EventId, CancellationToken.None))
                                      .ReturnsAsync(false);

            // Assert
            _ = await Assert.ThrowsAsync<NoAccessException>(async () => await _handler.Handle(query, CancellationToken.None));
        }
    }
}