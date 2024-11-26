using System.Threading;
using System.Threading.Tasks;

using Application.Queries.RSVPQueries;

using AutoMapper;

using Contracts.Models;

using Domain.Aggregates.Events;
using Domain.Exceptions;

using Infrastructure;

using Moq;

namespace UnitTests.Queries.RSVPQueries
{
    public class GetRSVPByIdQueryTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetRSVPByIdHandler _handler;

        public GetRSVPByIdQueryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetRSVPByIdHandler(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task HandleValidGetRSVPByIdQueryReturnsRSVP()
        {
            // Arrange
            GetRSVPByIdQuery query = new()
            {
                RSVPId = "rsvp123",
                UserId = "user123"
            };

            RSVPDTO rsvpDto = new()
            {
                Id = "rsvp123",
                UserId = "user123",
                EventId = "event123"
            };

            RSVP mappedRSVP = new()
            {
                Id = "rsvp123",
                UserEmail = "user123",
                EventTitle = "event123"
            };

            _ = _unitOfWorkMock.Setup(r => r.RSVP.GetRSVPById(query.RSVPId, CancellationToken.None))
                                      .ReturnsAsync(rsvpDto);

            _ = _unitOfWorkMock.Setup(r => r.Event.IsUserOwner(query.UserId, rsvpDto.EventId, CancellationToken.None))
                                      .ReturnsAsync(true);

            _ = _mapperMock.Setup(m => m.Map<RSVP>(rsvpDto))
                            .Returns(mappedRSVP);

            // Act
            GetRSVPByIdResult result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mappedRSVP, result.RSVP);
        }

        [Fact]
        public async Task HandleInvalidGetRSVPByIdQueryThrowsNoAccessException()
        {
            // Arrange
            GetRSVPByIdQuery query = new()
            {
                RSVPId = "rsvp123",
                UserId = "user123"
            };

            RSVPDTO rsvpDto = new()
            {
                Id = "rsvp123",
                UserId = "otherUser456",
                EventId = "event123"
            };

            _ = _unitOfWorkMock.Setup(r => r.RSVP.GetRSVPById(query.RSVPId, CancellationToken.None))
                                      .ReturnsAsync(rsvpDto);

            _ = _unitOfWorkMock.Setup(r => r.Event.IsUserOwner(query.UserId, rsvpDto.EventId, CancellationToken.None))
                                      .ReturnsAsync(false);

            _ = _unitOfWorkMock.Setup(r => r.RSVP.IsUserInvited(query.UserId, rsvpDto.EventId, CancellationToken.None))
                                      .ReturnsAsync(false);

            // Assert
            _ = await Assert.ThrowsAsync<NoAccessException>(async () => await _handler.Handle(query, CancellationToken.None));
        }
    }
}