using System.Threading;
using System.Threading.Tasks;

using Application.Commands.RSVPs;

using Domain.Aggregates.Events;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Interfaces;

using Infrastructure;

using Moq;

namespace UnitTests.Commands.RSVPCommands
{
    public class UpdateRSVPStatusCommandTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IRSVPRepository> _rsvpRepositoryMock;
        private readonly UpdateRSVPStatusHandler _handler;

        public UpdateRSVPStatusCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _rsvpRepositoryMock = new Mock<IRSVPRepository>();

            _ = _unitOfWorkMock.Setup(r => r.RSVP).Returns(_rsvpRepositoryMock.Object);

            _handler = new UpdateRSVPStatusHandler(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task HandleValidUpdateRSVPStatusRequestReturnsNotNullUpdateRSVPStatusResult()
        {
            // Arrange
            UpdateRSVPStatusCommand command = new()
            {
                RSVPAccepted = true,
                EventId = "rsvp123",
                UserId = "user123"
            };

            _ = _rsvpRepositoryMock.Setup(r => r.IsUserInvited(command.UserId, command.EventId, CancellationToken.None))
                                   .ReturnsAsync(true);

            // Act
            UpdateRSVPStatusResult result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task HandleInvalidUpdateRSVPStatusRequestThrowsExceptionWhenUserIsNotInvited()
        {
            // Arrange
            UpdateRSVPStatusCommand command = new()
            {
                RSVPAccepted = true,
                EventId = "rsvp123",
                UserId = "user123"
            };

            _ = _rsvpRepositoryMock.Setup(r => r.IsUserInvited(command.UserId, command.EventId, CancellationToken.None))
                                   .ReturnsAsync(false);

            // Assert
            _ = await Assert.ThrowsAsync<NoAccessException>(async () => await _handler.Handle(command, CancellationToken.None));
        }
        [Fact]
        public async Task HandleUpdateRSVPStatusCommandSetsNewStatusCorrectlyWhenAccepted()
        {
            // Arrange
            UpdateRSVPStatusCommand command = new()
            {
                RSVPAccepted = true,
                EventId = "rsvp123",
                UserId = "user123"
            };
            _ = _rsvpRepositoryMock.Setup(r => r.IsUserInvited(command.UserId, command.EventId, CancellationToken.None))
                                   .ReturnsAsync(true);
            // Act
            _ = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(r => r.RSVP.ChangeRSVPStatus(command.EventId, RSVPStatus.Accepted, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task HandleUpdateRSVPStatusCommandSetsNewStatusCorrectlyWhenDeclined()
        {
            // Arrange
            UpdateRSVPStatusCommand command = new()
            {
                RSVPAccepted = false,
                EventId = "rsvp456",
                UserId = "user456"
            };
            _ = _rsvpRepositoryMock.Setup(r => r.IsUserInvited(command.UserId, command.EventId, CancellationToken.None))
                                   .ReturnsAsync(true);
            // Act
            _ = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(r => r.RSVP.ChangeRSVPStatus(command.EventId, RSVPStatus.Declined, CancellationToken.None), Times.Once);
        }
    }
}