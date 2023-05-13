using Contracts.Models.Statuses;

namespace Contracts.Models
{
    public class RSPV
    {
        public string Id { get; init; }
        public string EventTitle { get; init; }
        public string UserEmail { get; init; }
        public RSPVStatus Status { get; init; }
    }
}