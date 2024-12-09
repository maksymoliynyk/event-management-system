using Domain.Entities.Users;
using Domain.Enums;

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

    public void ChangeStatus(RSVPStatus status, Guid userId)
    {
        if (UserId != userId)
        {
            throw new Exception("Cannot performed by not owner");
        }

        if (Status == RSVPStatus.Pending)
        {
            throw new Exception("Invalid status");
        }

        Status = status;
    }
}