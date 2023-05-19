using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Domain.Models.Database;

namespace Domain.Interfaces
{
    public interface IRSVPRepository
    {
        Task<RSVPDTO> SendRSVPToUser(EventDTO eventDTO, UserDTO userDTO, CancellationToken cancellationToken = default);
        Task<RSVPDTO> GetRSVPById(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<RSVPDTO>> GetAllRSVPsForEvent(string eventId, CancellationToken cancellationToken = default);
    }
}