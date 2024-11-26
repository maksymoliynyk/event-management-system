using System;

using Domain.Entities.Users;

namespace Domain.Aggregates.Events;

public class Attendee
{
    public Guid EventId { get; private set; }

    public Guid AttendeeId { get; private set; }

    // Navigation properties
    public Event Event { get; }
    public User AttendeeUser { get; }

    internal Attendee(Guid eventId, Guid attendeeId)
    {
        EventId = eventId;
        AttendeeId = attendeeId;
    }
}