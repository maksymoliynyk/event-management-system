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
    public class GetAllEventsCreatedByUserQueryTest
    {
        private readonly Mock<IRepositoryManager> _repositoryManagerMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IEventRepository> _eventRepositoryMock = new();
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllEventsCreatedByUserQueryHandler _getAllEventsCreatedByUserQueryHandler;
        public GetAllEventsCreatedByUserQueryTest()
        {
            _mapperMock = new Mock<IMapper>();
            _ = _repositoryManagerMock.Setup(x => x.User).Returns(_userRepositoryMock.Object);
            _ = _repositoryManagerMock.Setup(x => x.Event).Returns(_eventRepositoryMock.Object);
            _getAllEventsCreatedByUserQueryHandler = new GetAllEventsCreatedByUserQueryHandler(_repositoryManagerMock.Object, _mapperMock.Object);
        }
        [Fact]
        public async Task GetAllEventsCreatedByUserShouldReturnEvents()
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
            GetAllEventsCreatedByUserResult result = await _getAllEventsCreatedByUserQueryHandler.Handle(new GetAllEventsCreatedByUserQuery
            {
                Id = userId.ToString()
            }, default);
            // Assert
            _ = result.ShouldNotBeNull();
            result.Events.ShouldNotBeEmpty();
            _repositoryManagerMock.Verify(x => x.User.GetAllEventsCreatedByUser(It.IsAny<string>(), default), Times.Once);

        }

    }
}