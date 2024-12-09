using System;

using Domain.Aggregates.Events;
using Domain.Entities.Users;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class EventManagementContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Event> Events { get; set; }

    public EventManagementContext(DbContextOptions<EventManagementContext> options) : base(options) { }

    // for dotnet-ef
    public EventManagementContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("eventmanagement");
        builder.ApplyConfigurationsFromAssembly(typeof(EventManagementContext).Assembly);
    }
}