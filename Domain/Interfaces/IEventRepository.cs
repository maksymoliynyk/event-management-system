using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Contracts.Models.Statuses;

using Domain.Models.Database;

namespace Domain.Interfaces
{
    public interface IEventRepository
    {
        Task<string> CreateEvent(EventDTO newEvent, CancellationToken cancellationToken = default);
        Task<EventDTO> GetEventById(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<EventDTO>> GetEventsByOwner(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<EventDTO>> GetEventsByOwnerByCondition(string userId, Func<EventDTO, bool> condition, CancellationToken cancellationToken = default);
        Task<EventDTO> DeleteEventById(string id, CancellationToken cancellationToken = default);
        Task ChangeEventStatus(string id, EventStatus eventStatus, CancellationToken cancellationToken = default);
        Task UpdateEvent(EventDTO updatedEvent, CancellationToken cancellationToken = default);
    }
}