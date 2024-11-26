using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Application.Queries.RSVPQueries;

using AutoMapper;

using Contracts.Models;

using Domain.Aggregates.Events;

using Infrastructure;

using Moq;

namespace UnitTests.Queries.RSVPQueries
{
    public class GetUsersRSVPQueryTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetUsersRSVPHandler _handler;

        public GetUsersRSVPQueryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetUsersRSVPHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task HandleValidGetUsersRSVPQueryReturnsAllRSVPsForUser()
        {
            // Arrange
            GetUsersRSVPQuery query = new()
            {
                UserId = "user123"
            };

            IEnumerable<RSVPDTO> rsvpDtos = new List<RSVPDTO>
            {
                new RSVPDTO { Id = "rsvp1", UserId = "user123", EventId = "event1" },
                new RSVPDTO { Id = "rsvp2", UserId = "user123", EventId = "event2" },
                new RSVPDTO { Id = "rsvp3", UserId = "user123", EventId = "event3" }
            };

            IEnumerable<RSVP> mappedRSVPs = new List<RSVP>
            {
                new RSVP { Id = "rsvp1", UserEmail = "user123", EventTitle = "event1" },
                new RSVP { Id = "rsvp2", UserEmail = "user123", EventTitle = "event2" },
                new RSVP { Id = "rsvp3", UserEmail = "user123", EventTitle = "event3" }
            };

            _ = _unitOfWorkMock.Setup(r => r.RSVP.GetAllRSVPsForUser(query.UserId, CancellationToken.None))
                                      .ReturnsAsync(rsvpDtos);

            _ = _mapperMock.Setup(m => m.Map<IEnumerable<RSVP>>(rsvpDtos))
                            .Returns(mappedRSVPs);

            // Act
            GetUsersRSVPResult result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mappedRSVPs, result.RSVPs);
        }
    }
}