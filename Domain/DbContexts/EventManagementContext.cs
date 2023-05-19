using Domain.Models.Database;

using Microsoft.EntityFrameworkCore;

namespace Domain.DbContexts
{
    public class EventManagementContext : DbContext
    {
        public DbSet<UserDTO> Users { get; set; }
        public DbSet<EventDTO> Events { get; set; }
        public DbSet<RSVPDTO> RSPVs { get; set; }

        public EventManagementContext()
        {
        }
        public EventManagementContext(DbContextOptions<EventManagementContext> options) : base(options)
        {
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