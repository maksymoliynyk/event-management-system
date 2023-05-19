using Contracts.Models.Statuses;

namespace Contracts.Models
{
    public class RSVP
    {
        public string Id { get; init; }
        public string EventTitle { get; init; }
        public string UserEmail { get; init; }
        public RSVPStatus Status { get; init; }
    }
}