using System;

using Domain.Aggregates.Events;
using Domain.Entities.Users;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

internal class EventManagementContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Event> Events { get; set; }
    public DbSet<RSVP> RSPVs { get; set; }

    public EventManagementContext(DbContextOptions<EventManagementContext> options) : base(options) { }

    // for dotnet-ef
    public EventManagementContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("event_management");
        builder.ApplyConfigurationsFromAssembly(typeof(EventManagementContext).Assembly);
    }
}