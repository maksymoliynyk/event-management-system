using System.Threading;
using System.Threading.Tasks;

using Contracts.Models.Statuses;

using Domain.Commands.RSVPCommands;
using Domain.Exceptions;
using Domain.Interfaces;

using Moq;

namespace UnitTests.Commands.RSVPCommands
{
    public class UpdateRSVPStatusCommandTests
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock;
        private readonly Mock<IRSVPRepository> _rsvpRepositoryMock;
        private readonly UpdateRSVPStatusHandler _handler;

        public UpdateRSVPStatusCommandTests()
        {
            _repositoryManagerMock = new Mock<IRepositoryManager>();
            _rsvpRepositoryMock = new Mock<IRSVPRepository>();

            _ = _repositoryManagerMock.Setup(r => r.RSVP).Returns(_rsvpRepositoryMock.Object);

            _handler = new UpdateRSVPStatusHandler(_repositoryManagerMock.Object);
        }

        [Fact]
        public async Task HandleValidUpdateRSVPStatusRequestReturnsNotNullUpdateRSVPStatusResult()
        {
            // Arrange
            UpdateRSVPStatusCommand command = new()
            {
                RSVPAccepted = true,
                RSVPId = "rsvp123",
                UserId = "user123"
            };

            _ = _rsvpRepositoryMock.Setup(r => r.IsUserInvited(command.UserId, command.RSVPId, CancellationToken.None))
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
                RSVPId = "rsvp123",
                UserId = "user123"
            };

            _ = _rsvpRepositoryMock.Setup(r => r.IsUserInvited(command.UserId, command.RSVPId, CancellationToken.None))
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
                RSVPId = "rsvp123",
                UserId = "user123"
            };
            _ = _rsvpRepositoryMock.Setup(r => r.IsUserInvited(command.UserId, command.RSVPId, CancellationToken.None))
                                   .ReturnsAsync(true);
            // Act
            _ = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repositoryManagerMock.Verify(r => r.RSVP.ChangeRSVPStatus(command.RSVPId, RSVPStatus.Accepted, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task HandleUpdateRSVPStatusCommandSetsNewStatusCorrectlyWhenDeclined()
        {
            // Arrange
            UpdateRSVPStatusCommand command = new()
            {
                RSVPAccepted = false,
                RSVPId = "rsvp456",
                UserId = "user456"
            };
            _ = _rsvpRepositoryMock.Setup(r => r.IsUserInvited(command.UserId, command.RSVPId, CancellationToken.None))
                                   .ReturnsAsync(true);
            // Act
            _ = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _repositoryManagerMock.Verify(r => r.RSVP.ChangeRSVPStatus(command.RSVPId, RSVPStatus.Declined, CancellationToken.None), Times.Once);
        }
    }
}