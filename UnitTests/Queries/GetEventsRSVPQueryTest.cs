using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Queries;

using Faker;

using Moq;

using Shouldly;

namespace UnitTests.Queries
{
    public class GetEventsRSVPQueryTest
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IEventRepository> _eventRepositoryMock = new();
        private readonly Mock<IRSVPRepository> _rsvpRepositoryMock = new();
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetEventsRSVPQueryHandler _getEventsRSVPQueryHandler;
        public GetEventsRSVPQueryTest()
        {
            _mapperMock = new Mock<IMapper>();
            _ = _repositoryManagerMock.Setup(x => x.User).Returns(_userRepositoryMock.Object);
            _ = _repositoryManagerMock.Setup(x => x.Event).Returns(_eventRepositoryMock.Object);
            _ = _repositoryManagerMock.Setup(x => x.RSVP).Returns(_rsvpRepositoryMock.Object);
            _getEventsRSVPQueryHandler = new GetEventsRSVPQueryHandler(_repositoryManagerMock.Object, _mapperMock.Object);
        }
        [Fact]
        public async Task GetAllRSVPsForEventShouldReturnRSVPs()
        {
            // Arrange
            Guid eventId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            string userEmail = Internet.Email();
            string title = Lorem.Sentence();
            _ = _userRepositoryMock.Setup(x => x.GetUserByEmailOrCreateUser(It.IsAny<string>(), default)).ReturnsAsync(new UserDTO
            {
                Id = userId,
                Email = userEmail
            });
            _ = _eventRepositoryMock.Setup(x => x.GetEventById(It.IsAny<string>(), default)).ReturnsAsync(new EventDTO
            {
                Id = eventId,
                Title = title,
                OwnerId = userId
            });
            _ = _rsvpRepositoryMock.Setup(x => x.GetAllRSVPsForEvent(It.IsAny<string>(), default)).ReturnsAsync(new List<RSVPDTO>
            {
                new RSVPDTO
                {
                    UserId = userId,
                    EventId = eventId
                }
            });
            _ = _mapperMock.Setup(x => x.Map<IEnumerable<RSVP>>(It.IsAny<IEnumerable<RSVP>>(), default)).Returns(
                new List<RSVP>
                {
                    new RSVP
                    {
                        UserEmail = userEmail,
                        EventTitle = title
                    }
                }
            );
            // Act
            GetEventsRSVPResult result = await _getEventsRSVPQueryHandler.Handle(new GetEventsRSVPQuery
            {
                EventId = eventId.ToString()
            }, default);
            // Assert
            _ = result.ShouldNotBeNull();
            _ = result.RSVPs.ShouldNotBeNull();
            _repositoryManagerMock.Verify(x => x.RSVP.GetAllRSVPsForEvent(It.IsAny<string>(), default), Times.Once);
        }

    }
}