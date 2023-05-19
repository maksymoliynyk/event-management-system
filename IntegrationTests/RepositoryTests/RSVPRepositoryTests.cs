using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Domain.DbContexts;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Repositories;

using IntegrationTests.RepositoryTests.Helpers;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using Shouldly;

namespace IntegrationTests.RepositoryTests
{
    public class RSVPRepositoryTests
    {
        private readonly IRepositoryManager _repositoryManager;
        public RSVPRepositoryTests()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            _repositoryManager = new RepositoryManager(TestContext.GetEventManagementContext());
        }
        [Fact]
        public async Task SendRSVPToUserShouldCreateRSVP()
        {
            // Arrange
            using EventManagementContext context = TestContext.GetEventManagementContext();
            UserDTO userOwner = ArrangeHelper.ArrangeUser(context);
            UserDTO userToInvite = ArrangeHelper.ArrangeUser(context);
            EventDTO eventDTO = ArrangeHelper.ArrangeEvent(context, userOwner);

            // Act
            RSVPDTO rsvp = await _repositoryManager.RSVP.SendRSVPToUser(eventDTO, userToInvite, default);
            await _repositoryManager.SaveAsync(default);

            // Assert
            _ = rsvp.ShouldNotBeNull();
            rsvp.EventId.ShouldBe(eventDTO.Id);
            rsvp.UserId.ShouldBe(userToInvite.Id);

            using EventManagementContext newContext = TestContext.GetEventManagementContext();
            RSVPDTO rsvpFromDb = await newContext.RSPVs.FirstOrDefaultAsync(t => t.Id == rsvp.Id);
            _ = rsvpFromDb.ShouldNotBeNull();
            rsvpFromDb.EventId.ShouldBe(rsvp.EventId);
            rsvpFromDb.UserId.ShouldBe(rsvp.UserId);

            ArrangeHelper.CleanUp(context, rsvpFromDb);
            ArrangeHelper.CleanUp(context, eventDTO);
            ArrangeHelper.CleanUp(context, userToInvite);
            ArrangeHelper.CleanUp(context, userOwner);
        }
        [Fact]
        public async Task SendRSVPToUserShouldThrowExceptionWhenUserIsOwner()
        {
            // Arrange
            using EventManagementContext context = TestContext.GetEventManagementContext();
            UserDTO userOwner = ArrangeHelper.ArrangeUser(context);
            EventDTO eventDTO = ArrangeHelper.ArrangeEvent(context, userOwner);

            // Act
            async Task<RSVPDTO> sendRSVPToUser()
            {
                return await _repositoryManager.RSVP.SendRSVPToUser(eventDTO, userOwner, default);
            }

            // Assert
            _ = await Should.ThrowAsync<RSPVSendException>((Func<Task<RSVPDTO>>)sendRSVPToUser);
            ArrangeHelper.CleanUp(context, eventDTO);
            ArrangeHelper.CleanUp(context, userOwner);
        }
        [Fact]
        public async Task SendRSVPToUserShouldThrowExceptionWhenUserIsAlreadyInvited()
        {
            // Arrange
            using EventManagementContext context = TestContext.GetEventManagementContext();
            UserDTO userOwner = ArrangeHelper.ArrangeUser(context);
            UserDTO userToInvite = ArrangeHelper.ArrangeUser(context);
            EventDTO eventDTO = ArrangeHelper.ArrangeEvent(context, userOwner);
            _ = await context.RSPVs.AddAsync(new RSVPDTO
            {
                Id = Guid.NewGuid(),
                EventId = eventDTO.Id,
                UserId = userToInvite.Id
            });
            _ = context.SaveChanges();

            // Act
            async Task<RSVPDTO> sendRSVPToUser()
            {
                return await _repositoryManager.RSVP.SendRSVPToUser(eventDTO, userToInvite, default);
            }

            // Assert
            _ = await Should.ThrowAsync<RSPVSendException>((Func<Task<RSVPDTO>>)sendRSVPToUser);
            ArrangeHelper.CleanUp(context, eventDTO);
            ArrangeHelper.CleanUp(context, userToInvite);
            ArrangeHelper.CleanUp(context, userOwner);
        }

        [Fact]
        public async Task GetAllRSVPForEventShouldReturnRSVPs()
        {
            // Arrange
            using EventManagementContext context = TestContext.GetEventManagementContext();
            UserDTO userOwner = ArrangeHelper.ArrangeUser(context);
            UserDTO userToInvite1 = ArrangeHelper.ArrangeUser(context);
            UserDTO userToInvite2 = ArrangeHelper.ArrangeUser(context);
            EventDTO eventDTO = ArrangeHelper.ArrangeEvent(context, userOwner);
            EntityEntry<RSVPDTO> rsvp1 = context.RSPVs.Add(new RSVPDTO
            {
                Id = Guid.NewGuid(),
                EventId = eventDTO.Id,
                UserId = userToInvite1.Id
            });
            EntityEntry<RSVPDTO> rsvp2 = context.RSPVs.Add(new RSVPDTO
            {
                Id = Guid.NewGuid(),
                EventId = eventDTO.Id,
                UserId = userToInvite2.Id
            });
            _ = context.SaveChanges();

            // Act
            IEnumerable<RSVPDTO> rsvps = await _repositoryManager.RSVP.GetAllRSVPsForEvent(eventDTO.Id.ToString(), default);

            // Assert
            _ = rsvps.ShouldNotBeNull();
            rsvps.Count().ShouldBe(2);
            rsvps.Any(t => t.Id == rsvp1.Entity.Id).ShouldBeTrue();
            rsvps.Any(t => t.Id == rsvp2.Entity.Id).ShouldBeTrue();

            ArrangeHelper.CleanUp(context, rsvp1.Entity);
            ArrangeHelper.CleanUp(context, rsvp2.Entity);
            ArrangeHelper.CleanUp(context, eventDTO);
            ArrangeHelper.CleanUp(context, userToInvite1);
            ArrangeHelper.CleanUp(context, userToInvite2);
            ArrangeHelper.CleanUp(context, userOwner);
        }
    }
}