using Domain.Aggregates.Events;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class RSVPConfiguration : IEntityTypeConfiguration<RSVP>
{
    public void Configure(EntityTypeBuilder<RSVP> builder)
    {
        // Table Name
        builder.ToTable("RSVPs");

        // Primary Key
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.EventId)
            .IsRequired();

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(r => r.CreateDate)
            .IsRequired();

        // Navigation Properties
        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId);

        // Indexes
        builder.HasIndex(r => r.EventId).HasDatabaseName("IX_RSVP_EventId");
        builder.HasIndex(r => r.UserId).HasDatabaseName("IX_RSVP_UserId");
        builder.HasIndex(r => new { r.EventId, r.UserId }).HasDatabaseName("IX_RSVP_EventId_UserId").IsUnique();
    }
}
