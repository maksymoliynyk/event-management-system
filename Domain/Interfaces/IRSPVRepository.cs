using System.Threading;
using System.Threading.Tasks;

using Domain.Models.Database;

namespace Domain.Interfaces
{
    public interface IRSPVRepository
    {
        Task<string> SendRSPVToUser(EventDTO eventDTO, UserDTO userDTO, CancellationToken cancellationToken = default);
        Task<RSPVDTO> GetRSPVById(string id, CancellationToken cancellationToken = default);
    }
}