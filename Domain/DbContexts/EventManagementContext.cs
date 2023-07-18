using System;

using Domain.Models.Database;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Domain.DbContexts
{
    public class EventManagementContext : IdentityDbContext<UserDTO>
    {
        public DbSet<EventDTO> Events { get; set; }
        public DbSet<RSVPDTO> RSPVs { get; set; }

        public EventManagementContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public EventManagementContext(DbContextOptions<EventManagementContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                _ = optionsBuilder.UseNpgsql("Host=localhost;Database=EventManagement;Username=postgres;Password=db_learn");
            }
        }
    }
}