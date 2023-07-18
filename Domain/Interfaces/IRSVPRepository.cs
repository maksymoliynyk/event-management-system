using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Contracts.Models.Statuses;

using Domain.Models.Database;

namespace Domain.Interfaces
{
    public interface IRSVPRepository
    {
        Task<RSVPDTO> SendRSVPToUser(string eventId, string userId, CancellationToken cancellationToken = default);
        Task<RSVPDTO> GetRSVPById(string id, CancellationToken cancellationToken = default);
        Task ChangeRSVPStatus(string id, RSVPStatus rsvpStatus, CancellationToken cancellationToken = default);
        Task<IEnumerable<RSVPDTO>> GetAllRSVPsForEvent(string eventId, CancellationToken cancellationToken = default);
        Task<IEnumerable<RSVPDTO>> GetAllRSVPsForUser(string userId, CancellationToken cancellationToken = default);
        Task<bool> IsUserInvited(string userId, string eventId, CancellationToken cancellationToken = default);
    }
}