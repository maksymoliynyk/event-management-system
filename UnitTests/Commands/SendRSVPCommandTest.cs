using System;
using System.Threading.Tasks;

using Domain.Commands;
using Domain.Interfaces;

using Moq;
using Faker;
using Domain.Models.Database;
using Shouldly;

namespace UnitTests.Commands
{
    public class SendRSVPCommandTest
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new();
        private readonly Mock<IEventRepository> _eventRepositoryMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IRSVPRepository> _rsvpRepositoryMock = new();
        private readonly SendRSVPCommandHandler _sendRSVPCommandHandler;
        public SendRSVPCommandTest()
        {
            _ = _repositoryManagerMock.Setup(x => x.Event).Returns(_eventRepositoryMock.Object);
            _ = _repositoryManagerMock.Setup(x => x.User).Returns(_userRepositoryMock.Object);
            _ = _repositoryManagerMock.Setup(x => x.RSVP).Returns(_rsvpRepositoryMock.Object);
            _sendRSVPCommandHandler = new SendRSVPCommandHandler(_repositoryManagerMock.Object);
        }
        [Fact]
        public async Task SendRSVPShouldSendRSVP()
        {
            // Arrange
            Guid eventId = Guid.NewGuid();
            string userEmail = Internet.Email();
            Guid userId = Guid.NewGuid();
            Guid rsvpId = Guid.NewGuid();
            _ = _eventRepositoryMock.Setup(x => x.GetEventById(eventId.ToString(), default)).ReturnsAsync(new EventDTO
            {
                Id = eventId,
                Status = 0
            });
            _ = _userRepositoryMock.Setup(x => x.GetUserByEmailOrCreateUser(userEmail, default)).ReturnsAsync(new UserDTO
            {
                Id = userId,
                Email = userEmail
            });
            _ = _rsvpRepositoryMock.Setup(x => x.SendRSVPToUser(It.IsAny<EventDTO>(), It.IsAny<UserDTO>(), default)).ReturnsAsync(new RSVPDTO
            {
                Id = rsvpId,
                EventId = eventId,
                UserId = userId
            });
            // Act
            SendRSVPResult result = await _sendRSVPCommandHandler.Handle(new SendRSVPCommand
            {
                EventId = eventId.ToString(),
                UserEmail = userEmail
            }, default);
            // Assert
            _eventRepositoryMock.Verify(x => x.GetEventById(eventId.ToString(), default), Times.Once);
            _userRepositoryMock.Verify(x => x.GetUserByEmailOrCreateUser(userEmail, default), Times.Once);
            _rsvpRepositoryMock.Verify(x => x.SendRSVPToUser(It.IsAny<EventDTO>(), It.IsAny<UserDTO>(), default), Times.Once);
            _ = result.ShouldNotBeNull();
        }
    }
}