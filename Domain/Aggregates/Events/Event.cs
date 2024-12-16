using System.Collections.Generic;
using System.Linq;

using Domain.Entities.Users;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Aggregates.Events;

public class Event
{
    // this field has strange naming convention to match backing field conventions for EF configurations
    private readonly List<RSVP> _rSVPs = [];
    private readonly List<Attendee> _attendees = [];
    
    public static readonly string AttendeesBackingFieldName = nameof(_attendees);
    public static readonly string RsvpsBackingFieldName = nameof(_rSVPs);

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
    public ICollection<RSVP> RSVPs => _rSVPs.AsReadOnly();
    public ICollection<Attendee> Attendees => _attendees.AsReadOnly();

    public static Event CreateEvent(string title, string description, TimeSpan duration, string location, Guid ownerId,
        DateTime startDate)
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

    public void CancelEvent()
    {
        IsEventActive();

        Status = EventStatus.Cancelled;
    }

    public Guid CreateRSVP(Guid inviteeId)
    {
        IsEventActive();

        if (inviteeId == OwnerId)
        {
            throw new ActionsNotAllowedException(EntitiesErrorType.RSVP, "Owner cannot be invited");
        }

        if (RSVPs.FirstOrDefault(i => i.UserId == inviteeId) != null)
        {
            throw new ActionsNotAllowedException(EntitiesErrorType.RSVP, "User already invited");
        }

        var rsvp = new RSVP(Id, inviteeId);
        _rSVPs.Add(rsvp);

        return rsvp.Id;
    }

    private void IsEventActive()
    {
        if (Status == EventStatus.Cancelled)
        {
            throw new ActionsNotAllowedException(EntitiesErrorType.Event, "Event already cancelled");
        }

        if (StartDate.Add(Duration) < DateTime.UtcNow || Status == EventStatus.Finished)
        {
            throw new ActionsNotAllowedException(EntitiesErrorType.Event, "Event already ended");
        }
    }

    public void RespondToRsvp(RSVPStatus status, Guid userId)
    {
        var rsvp = RSVPs.FirstOrDefault(r => r.UserId == userId);
        if (rsvp == null)
        {
            throw new ObjectNotFoundException(EntitiesErrorType.RSVP);
        }

        rsvp.ChangeStatus(status);
        if (status == RSVPStatus.Accepted)
        {
            CreateAttendee(userId);
        }
    }

    private void CreateAttendee(Guid userId)
    {
        var attendee = new Attendee(Id, userId);
        _attendees.Add(attendee);
    }
}