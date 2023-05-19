using System.Threading.Tasks;

using Domain.DbContexts;
using Domain.Interfaces;
using Domain.Repositories;

using Faker;
using Shouldly;
using Domain.Models.Database;
using IntegrationTests.RepositoryTests.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

namespace IntegrationTests.RepositoryTests
{
    public class UserRepositoryTests
    {
        private readonly IRepositoryManager _repositoryManager;
        public UserRepositoryTests()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            _repositoryManager = new RepositoryManager(TestContext.GetEventManagementContext());
        }
        [Fact]
        public async Task GetUserByEmailOrCreateUserShouldReturnUserIfUserExists()
        {
            // Arrange

            using EventManagementContext context = TestContext.GetEventManagementContext();
            UserDTO newUser = ArrangeHelper.ArrangeUser(context);

            // Act
            UserDTO user = await _repositoryManager.User.GetUserByEmailOrCreateUser(newUser.Email, default);

            // Assert
            _ = user.ShouldNotBeNull();
            user.Id.ShouldBe(newUser.Id);
            user.Email.ShouldBe(newUser.Email);
            using EventManagementContext newContext = TestContext.GetEventManagementContext();
            ArrangeHelper.CleanUp(newContext, newUser);
        }

        [Fact]
        public async Task GetUserByEmailOrCreateUserShouldCreateUserIfUserDoesNotExist()
        {
            // Arrange
            string email = Internet.Email();

            // Act
            UserDTO user = await _repositoryManager.User.GetUserByEmailOrCreateUser(email, default);
            await _repositoryManager.SaveAsync(default);

            // Assert
            _ = user.ShouldNotBeNull();
            user.Email.ShouldBe(email);

            using EventManagementContext context = TestContext.GetEventManagementContext();
            UserDTO userFromDb = await context.Users.FirstOrDefaultAsync(t => t.Id == user.Id, default);
            ArrangeHelper.CleanUp(context, userFromDb);
            _ = userFromDb.ShouldNotBeNull();
            userFromDb.Id.ShouldBe(user.Id);
            userFromDb.Email.ShouldBe(user.Email);
        }
        [Fact]
        public async Task GetUserByIdShouldReturnUser()
        {
            // Arrange
            using EventManagementContext context = TestContext.GetEventManagementContext();
            UserDTO newUser = ArrangeHelper.ArrangeUser(context);

            // Act
            UserDTO user = await _repositoryManager.User.GetUserById(newUser.Id.ToString(), default);

            // Assert
            _ = user.ShouldNotBeNull();
            user.Id.ShouldBe(newUser.Id);
            user.Email.ShouldBe(newUser.Email);
            using EventManagementContext newContext = TestContext.GetEventManagementContext();
            ArrangeHelper.CleanUp(newContext, newUser);
        }
        [Fact]
        public async Task GetAllEventsCreatedByUserShouldReturnEvents()
        {
            // Arrange
            using EventManagementContext context = TestContext.GetEventManagementContext();
            UserDTO newUser = ArrangeHelper.ArrangeUser(context);
            EventDTO newEvent1 = ArrangeHelper.ArrangeEvent(context, newUser);
            EventDTO newEvent2 = ArrangeHelper.ArrangeEvent(context, newUser);

            // Act
            IEnumerable<EventDTO> events = await _repositoryManager.User.GetAllEventsCreatedByUser(newUser.Id.ToString(), default);

            // Assert
            _ = events.ShouldNotBeNull();
            events.Count().ShouldBe(2);
            events.ShouldContain(x => x.Id == newEvent1.Id);
            events.ShouldContain(x => x.Id == newEvent2.Id);
            using EventManagementContext newContext = TestContext.GetEventManagementContext();
            ArrangeHelper.CleanUp(newContext, newEvent1);
            ArrangeHelper.CleanUp(newContext, newEvent2);
            ArrangeHelper.CleanUp(newContext, newUser);
        }
        [Fact]
        public async Task GetEventsCreatedByUserByConditionWillReturnEventsByCondition()
        {
            // Arrange
            using EventManagementContext context = TestContext.GetEventManagementContext();
            UserDTO newUser = ArrangeHelper.ArrangeUser(context);
            EventDTO newEvent1 = ArrangeHelper.ArrangeEvent(context, newUser);
            EventDTO newEvent2 = ArrangeHelper.ArrangeEventYesterday(context, newUser);

            // Act
            IEnumerable<EventDTO> events = await _repositoryManager.User.GetEventsCreatedByUserByCondition(newUser.Id.ToString(), x => (x.Date + x.Duration) > DateTime.Now, default);

            // Assert
            _ = events.ShouldNotBeNull();
            events.Count().ShouldBe(1);
            events.ShouldContain(x => x.Id == newEvent1.Id);
            events.ShouldNotContain(x => x.Id == newEvent2.Id);
            using EventManagementContext newContext = TestContext.GetEventManagementContext();
            ArrangeHelper.CleanUp(newContext, newEvent1);
            ArrangeHelper.CleanUp(newContext, newEvent2);
            ArrangeHelper.CleanUp(newContext, newUser);
        }
    }
}