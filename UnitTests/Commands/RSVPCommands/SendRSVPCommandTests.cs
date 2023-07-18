using System.Threading;
using System.Threading.Tasks;

using Domain.Commands.RSVPCommands;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models.Database;

using Moq;

namespace UnitTests.Commands.RSVPCommands
{
    public class SendRSVPCommandTests
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly Mock<IRSVPRepository> _rsvpRepositoryMock;
        private readonly SendRSVPCommandHandler _handler;

        public SendRSVPCommandTests()
        {
            _repositoryManagerMock = new Mock<IRepositoryManager>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _rsvpRepositoryMock = new Mock<IRSVPRepository>();

            _ = _repositoryManagerMock.Setup(r => r.User).Returns(_userRepositoryMock.Object);
            _ = _repositoryManagerMock.Setup(r => r.Event).Returns(_eventRepositoryMock.Object);
            _ = _repositoryManagerMock.Setup(r => r.RSVP).Returns(_rsvpRepositoryMock.Object);

            _handler = new SendRSVPCommandHandler(_repositoryManagerMock.Object);
        }

        [Fact]
        public async Task HandleValidRSVPRequestReturnsNotNullSendRSVPResult()
        {
            // Arrange
            SendRSVPCommand command = new()
            {
                UserOwnerId = "owner123",
                UserInviteEmail = "user@example.com",
                EventId = "event123"
            };

            UserDTO user = new() { Id = "user123" };
            EventDTO searchedEvent = new() { Id = "event123", IsPublic = true };

            _ = _userRepositoryMock.Setup(u => u.GetUserByEmail(command.UserInviteEmail, CancellationToken.None))
                                  .ReturnsAsync(user);
            _ = _eventRepositoryMock.Setup(e => e.GetEventById(command.EventId, CancellationToken.None))
                                    .ReturnsAsync(searchedEvent);
            _ = _eventRepositoryMock.Setup(e => e.IsUserOwner(user.Id, searchedEvent.Id, CancellationToken.None))
                                    .ReturnsAsync(false);
            _ = _rsvpRepositoryMock.Setup(r => r.IsUserInvited(user.Id, searchedEvent.Id, CancellationToken.None))
                                   .ReturnsAsync(false);
            RSVPDTO createdRsvp = new() { Id = "rsvp123" };
            _ = _rsvpRepositoryMock.Setup(r => r.SendRSVPToUser(command.EventId, user.Id, CancellationToken.None))
                                   .ReturnsAsync(createdRsvp);

            // Act
            SendRSVPResult result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdRsvp.Id, result.RSVPId);
        }
        [Fact]
        public async Task HandleValidPrivateRSVPRequestReturnsNotNullSendRSVPResult()
        {
            // Arrange
            SendRSVPCommand command = new()
            {
                UserOwnerId = "owner123",
                UserInviteEmail = "user@example.com",
                EventId = "event123"
            };

            UserDTO user = new() { Id = "user123" };
            EventDTO searchedEvent = new() { Id = "event123", OwnerId = "owner123", IsPublic = false };

            _ = _userRepositoryMock.Setup(u => u.GetUserByEmail(command.UserInviteEmail, CancellationToken.None))
                                  .ReturnsAsync(user);
            _ = _eventRepositoryMock.Setup(e => e.GetEventById(command.EventId, CancellationToken.None))
                                    .ReturnsAsync(searchedEvent);
            _ = _eventRepositoryMock.Setup(e => e.IsUserOwner(user.Id, searchedEvent.Id, CancellationToken.None))
                                    .ReturnsAsync(false);
            _ = _rsvpRepositoryMock.Setup(r => r.IsUserInvited(user.Id, searchedEvent.Id, CancellationToken.None))
                                   .ReturnsAsync(false);
            RSVPDTO createdRsvp = new() { Id = "rsvp123" };
            _ = _rsvpRepositoryMock.Setup(r => r.SendRSVPToUser(command.EventId, user.Id, CancellationToken.None))
                                   .ReturnsAsync(createdRsvp);

            // Act
            SendRSVPResult result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createdRsvp.Id, result.RSVPId);
        }

        [Fact]
        public async Task HandleRSVPRequestThrowsExceptionWhenEventIsNotPublicAndUserIsNotOwner()
        {
            // Arrange
            SendRSVPCommand command = new()
            {
                UserOwnerId = "owner123",
                UserInviteEmail = "user@example.com",
                EventId = "event123"
            };

            UserDTO user = new() { Id = "user123" };
            EventDTO searchedEvent = new() { Id = "event123", IsPublic = false, OwnerId = "otherOwner123" };

            _ = _userRepositoryMock.Setup(u => u.GetUserByEmail(command.UserInviteEmail, CancellationToken.None))
                                  .ReturnsAsync(user);
            _ = _eventRepositoryMock.Setup(e => e.GetEventById(command.EventId, CancellationToken.None))
                                    .ReturnsAsync(searchedEvent);
            _ = _eventRepositoryMock.Setup(e => e.IsUserOwner(user.Id, searchedEvent.Id, CancellationToken.None))
                                    .ReturnsAsync(false);

            // Assert
            _ = await Assert.ThrowsAsync<NoAccessException>(async () => await _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task HandleRSVPRequestThrowsExceptionWhenUserIsOwnerOfEvent()
        {
            // Arrange
            SendRSVPCommand command = new()
            {
                UserOwnerId = "owner123",
                UserInviteEmail = "user@example.com",
                EventId = "event123"
            };

            UserDTO user = new() { Id = "user123" };
            EventDTO searchedEvent = new() { Id = "event123", IsPublic = true, OwnerId = "owner123" };

            _ = _userRepositoryMock.Setup(u => u.GetUserByEmail(command.UserInviteEmail, CancellationToken.None))
                                  .ReturnsAsync(user);
            _ = _eventRepositoryMock.Setup(e => e.GetEventById(command.EventId, CancellationToken.None))
                                    .ReturnsAsync(searchedEvent);
            _ = _eventRepositoryMock.Setup(e => e.IsUserOwner(user.Id, searchedEvent.Id, CancellationToken.None))
                                    .ReturnsAsync(true);

            // Assert
            RSPVSendException exception = await Assert.ThrowsAsync<RSPVSendException>(async () => await _handler.Handle(command, CancellationToken.None));
            Assert.Equal(RSPVSendExceptionError.UserIsOwner, exception.Error);
        }

        [Fact]
        public async Task HandleRSVPRequestThrowsExceptionWhenUserIsAlreadyInvited()
        {
            // Arrange
            SendRSVPCommand command = new()
            {
                UserOwnerId = "owner123",
                UserInviteEmail = "user@example.com",
                EventId = "event123"
            };

            UserDTO user = new() { Id = "user123" };
            EventDTO searchedEvent = new() { Id = "event123", IsPublic = true, OwnerId = "otherOwner123" };

            _ = _userRepositoryMock.Setup(u => u.GetUserByEmail(command.UserInviteEmail, CancellationToken.None))
                                  .ReturnsAsync(user);
            _ = _eventRepositoryMock.Setup(e => e.GetEventById(command.EventId, CancellationToken.None))
                                    .ReturnsAsync(searchedEvent);
            _ = _eventRepositoryMock.Setup(e => e.IsUserOwner(user.Id, searchedEvent.Id, CancellationToken.None))
                                    .ReturnsAsync(false);
            _ = _rsvpRepositoryMock.Setup(r => r.IsUserInvited(user.Id, searchedEvent.Id, CancellationToken.None))
                                   .ReturnsAsync(true);

            // Assert
            _ = await Assert.ThrowsAsync<RSPVSendException>(async () => await _handler.Handle(command, CancellationToken.None));
            RSPVSendException exception = await Assert.ThrowsAsync<RSPVSendException>(async () => await _handler.Handle(command, CancellationToken.None));
            Assert.Equal(RSPVSendExceptionError.UserAlreadyRSVPd, exception.Error);
        }
    }
}
