using System;
using System.Collections.Generic;

using Domain.Entities.Users;
using Domain.Enums;

namespace Domain.Aggregates.Events;

public class Event
{
    private readonly List<RSVP> _rsvps = [];
    private readonly List<Attendee> _attendees = [];

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public TimeSpan Duration { get; private set; }
    public string Location { get; private set; }
    public Guid OwnerId { get; private set; }
    public EventStatus Status { get; private set; }
    public DateTime CreateDate { get; private set; }

    // Navigation properties
    public User Owner { get; }
    public IReadOnlyCollection<RSVP> RSVPs => _rsvps.AsReadOnly();
    public IReadOnlyCollection<Attendee> Attendees => _attendees.AsReadOnly();

    public static Event CreateEvent(string title, string description, TimeSpan duration, string location, Guid ownerId, DateTime startDate)
    {
        var newEvent = new Event()
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Duration = duration,
            Location = location,
            OwnerId = ownerId,
            Status = EventStatus.InProgress,
            StartDate = startDate,
            CreateDate = DateTime.UtcNow
        };

        return newEvent;
    }

    public void CreateRSVP(Guid inviteeId)
    {
        if (inviteeId == OwnerId)
        {
            throw new Exception("Owner cannot be invited");
        }

        if (Status == EventStatus.Cancelled)
        {
            throw new Exception("Event already cancelled");
        }

        if (StartDate.Add(Duration) > DateTime.UtcNow || Status == EventStatus.Finished)
        {
            throw new Exception("Event already ended");
        }

        var rsvp = new RSVP(Id, inviteeId);
        _rsvps.Add(rsvp);
    }

    public void CreateAttendee(Guid userId)
    {
        var attendee = new Attendee(Id, userId);
        _attendees.Add(attendee);
    }
}