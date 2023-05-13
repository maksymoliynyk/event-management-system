using System.Threading;
using System.Threading.Tasks;

using Contracts.Models.Statuses;

using Domain.Models.Database;

namespace Domain.Repositories
{
    public interface IRepository
    {
        Task<UserDTO> GetUserByEmailOrCreateUser(string email, CancellationToken cancellationToken = default);
        Task<string> CreateEvent(EventDTO newEvent, CancellationToken cancellationToken = default);
        Task<EventDTO> GetEventById(string id, CancellationToken cancellationToken = default);
        Task<EventDTO> DeleteEventById(string id, CancellationToken cancellationToken = default);
        Task ChangeEventStatus(string id, EventStatus eventStatus, CancellationToken cancellationToken = default);
        Task UpdateEvent(EventDTO updatedEvent, CancellationToken cancellationToken = default);
    }
}