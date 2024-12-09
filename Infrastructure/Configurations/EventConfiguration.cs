using Domain.Aggregates.Events;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        // Table Name
        builder.ToTable("Events");

        // Primary Key
        builder.HasKey(e => e.Id);

        // Properties
        builder.Property(e => e.Id)
            .ValueGeneratedNever();

        builder.Property(e => e.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.StartDate)
            .IsRequired();

        builder.Property(e => e.Duration)
            .IsRequired();

        builder.Property(e => e.Location)
            .HasMaxLength(100);

        builder.Property(e => e.Status)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(e => e.CreateDate)
            .IsRequired();

        // Navigation Properties
        builder.HasOne(e => e.Owner)
            .WithMany()
            .HasForeignKey(e => e.OwnerId)
            .IsRequired();

        builder.HasMany(e => e.RSVPs)
            .WithOne()
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasMany(e => e.Attendees)
            .WithOne()
            .HasForeignKey(a => a.EventId)
            .OnDelete(DeleteBehavior.ClientCascade);

        // Indexes
        builder.HasIndex(e => e.OwnerId).HasDatabaseName("IX_Event_OwnerId");
        builder.HasIndex(e => new { e.Id, e.OwnerId }).HasDatabaseName("IX_Event_Id_OwnerId").IsUnique();
    }
}