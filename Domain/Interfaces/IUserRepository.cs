using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Domain.Models.Database;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDTO> GetUserByEmailOrCreateUser(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<EventDTO>> GetAllEventsCreatedByUser(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<EventDTO>> GetEventsCreatedByUserByCondition(string id, Func<EventDTO, bool> condition, CancellationToken cancellationToken = default);
    }
}