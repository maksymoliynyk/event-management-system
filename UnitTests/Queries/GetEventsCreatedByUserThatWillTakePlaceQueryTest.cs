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
    public class GetEventsCreatedByUserThatWillTakePlaceQueryTest
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IEventRepository> _eventRepositoryMock = new();
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetEventsCreatedByUserThatWillTakePlaceQueryHandler _getEventsCreatedByUserThatWillTakePlaceQueryHandler;
        public GetEventsCreatedByUserThatWillTakePlaceQueryTest()
        {
            _mapperMock = new Mock<IMapper>();
            _ = _repositoryManagerMock.Setup(x => x.User).Returns(_userRepositoryMock.Object);
            _ = _repositoryManagerMock.Setup(x => x.Event).Returns(_eventRepositoryMock.Object);
            _getEventsCreatedByUserThatWillTakePlaceQueryHandler = new GetEventsCreatedByUserThatWillTakePlaceQueryHandler(_repositoryManagerMock.Object, _mapperMock.Object);
        }
        [Fact]
        public async Task GetEventsCreatedByUserThatWillTakePlaceShouldReturnEvents()
        {
            // Arrange
            Guid eventId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            string userEmail = Internet.Email();
            _ = _mapperMock.Setup(x => x.Map<IEnumerable<Event>>(It.IsAny<IEnumerable<EventDTO>>())).Returns(new Event[]{
                new Event
                {
                    Id = eventId.ToString(),
                    OwnerEmail = userEmail
                }
                });
            // Act
            GetEventsCreatedByUserThatWillTakePlaceResult result = await _getEventsCreatedByUserThatWillTakePlaceQueryHandler.Handle(new GetEventsCreatedByUserThatWillTakePlaceQuery
            {
                Id = userId.ToString()
            }, default);
            // Assert
            _ = result.ShouldNotBeNull();
            result.Events.ShouldNotBeEmpty();
            _repositoryManagerMock.Verify(x => x.User.GetEventsCreatedByUserByCondition(It.IsAny<string>(), It.IsAny<Func<EventDTO, bool>>(), default), Times.Once);

        }

    }
}