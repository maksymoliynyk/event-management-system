using Domain.Aggregates.Events;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class AttendeeConfiguration : IEntityTypeConfiguration<Attendee>
{
    public void Configure(EntityTypeBuilder<Attendee> builder)
    {
        // Table Name
        builder.ToTable("Attendees");

        builder.Property(r => r.EventId)
            .IsRequired();

        builder.Property(r => r.AttendeeId)
            .IsRequired();

        builder.HasIndex(a => new { a.EventId, a.AttendeeId }).HasDatabaseName("IX_Attendee_EventId_AttendeeId").IsUnique();
    }
}