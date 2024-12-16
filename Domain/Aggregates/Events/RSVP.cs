using Domain.Entities.Users;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Aggregates.Events;

public class RSVP
{
    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public Guid UserId { get; private set; }
    public RSVPStatus Status { get; private set; }
    public DateTime CreateDate { get; private set; }

    // Navigation properties
    public Event Event { get; }
    public User User { get; }

    internal RSVP(Guid eventId, Guid userId)
    {
        EventId = eventId;
        UserId = userId;

        Id = Guid.NewGuid();
        Status = RSVPStatus.Pending;
        CreateDate = DateTime.UtcNow;
    }

    internal void ChangeStatus(RSVPStatus status)
    {
        if (status == RSVPStatus.Pending || Status != RSVPStatus.Pending)
        {
            throw new ActionsNotAllowedException(EntitiesErrorType.RSVP, "Invalid status");
        }

        Status = status;
    }
}