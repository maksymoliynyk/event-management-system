using System;

using Domain.DbContexts;
using Domain.Models.Database;

using Faker;

namespace IntegrationTests.RepositoryTests.Helpers
{
    public static class ArrangeHelper
    {
        public static UserDTO ArrangeUser(EventManagementContext context)
        {
            UserDTO user = new()
            {
                Id = Guid.NewGuid(),
                Email = Internet.Email()
            };
            _ = context.Users.Add(user);
            _ = context.SaveChanges();
            return user;
        }
        public static EventDTO ArrangeEvent(EventManagementContext context, UserDTO user)
        {
            EventDTO newEvent = new()
            {
                Id = Guid.NewGuid(),
                Title = Lorem.Sentence(),
                Description = Lorem.Paragraph(),
                Location = Lorem.Sentence(),
                Date = DateTime.Now,
                Duration = TimeSpan.FromHours(1),
                OwnerId = user.Id,
                Status = 0
            };
            _ = context.Events.Add(newEvent);
            _ = context.SaveChanges();
            return newEvent;
        }
        public static EventDTO ArrangeEventYesterday(EventManagementContext context, UserDTO user)
        {
            EventDTO newEvent = new()
            {
                Id = Guid.NewGuid(),
                Title = Lorem.Sentence(),
                Description = Lorem.Paragraph(),
                Location = Lorem.Sentence(),
                Date = DateTime.Now.AddDays(-1),
                Duration = TimeSpan.FromHours(1),
                OwnerId = user.Id,
                Status = 0
            };
            _ = context.Events.Add(newEvent);
            _ = context.SaveChanges();
            return newEvent;
        }
        public static void CleanUp(EventManagementContext context, UserDTO user, EventDTO newEvent)
        {
            _ = context.Users.Remove(user);
            _ = context.Events.Remove(newEvent);
            _ = context.SaveChanges();
        }
        public static void CleanUp(EventManagementContext context, UserDTO user)
        {
            _ = context.Users.Remove(user);
            _ = context.SaveChanges();
        }
        public static void CleanUp(EventManagementContext context, EventDTO newEvent)
        {
            _ = context.Events.Remove(newEvent);
            _ = context.SaveChanges();
        }
        public static void CleanUp(EventManagementContext context, RSVPDTO rsvpDTO)
        {
            _ = context.RSPVs.Remove(rsvpDTO);
            _ = context.SaveChanges();
        }
    }
}