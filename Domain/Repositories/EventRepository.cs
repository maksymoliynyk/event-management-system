using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Contracts.Models.Statuses;

using Domain.DbContexts;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models.Database;

using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventManagementContext _context;

        public EventRepository(EventManagementContext context)
        {
            _context = context;
        }
        public async Task ChangeEventStatus(string id, EventStatus eventStatus, CancellationToken cancellationToken = default)
        {
            EventDTO eventToChange = await GetEventById(id, cancellationToken);
            if (eventToChange.Status is 1 or 2)
            {
                throw new EventStatusException(EventStatusError.StatusCannotBeChanged, $"Event status cannot be changed");
            }
            if (eventToChange.Status == (int)eventStatus)
            {
                throw new EventStatusException(EventStatusError.NewStatusCannotBeSameAsOldStatus, $"Event status is already {eventStatus}");
            }
            eventToChange.Status = (int)eventStatus;
            return;
        }

        public async Task<string> CreateEvent(EventDTO newEvent, CancellationToken cancellationToken = default)
        {
            _ = await _context.Events.AddAsync(newEvent, cancellationToken);
            return newEvent.Id.ToString();
        }

        public async Task<EventDTO> DeleteEventById(string id, CancellationToken cancellationToken = default)
        {
            EventDTO eventToDelete = await _context.Events.FirstOrDefaultAsync(t => t.Id.ToString() == id, cancellationToken);
            _ = _context.Events.Remove(eventToDelete);
            return eventToDelete;
        }

        public async Task<EventDTO> GetEventById(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Events
                            .Include(t => t.Owner)
                            .FirstOrDefaultAsync(t => t.Id.ToString() == id, cancellationToken);
        }

        public Task<IEnumerable<EventDTO>> GetEventsByOwner(string userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_context.Events
                            .Include(t => t.Owner)
                            .Where(t => t.OwnerId == userId)
                            .AsEnumerable());
        }

        public Task<IEnumerable<EventDTO>> GetEventsByOwnerByCondition(string userId, Func<EventDTO, bool> condition, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_context.Events
                            .Include(t => t.Owner)
                            .Where(t => t.OwnerId == userId)
                            .Where(condition)
                            .AsEnumerable());
        }

        //? This method is not used in the application, but it can be useful in future

        public async Task UpdateEvent(EventDTO updatedEvent, CancellationToken cancellationToken = default)
        {
            EventDTO eventToUpdate = await GetEventById(updatedEvent.Id.ToString(), cancellationToken);
            if (eventToUpdate == null)
            {
                return;
            }
            eventToUpdate.Id = updatedEvent.Id;
            eventToUpdate.Title = updatedEvent.Title;
            eventToUpdate.Description = updatedEvent.Description;
            eventToUpdate.Date = updatedEvent.Date;
            eventToUpdate.Duration = updatedEvent.Duration;
            eventToUpdate.OwnerId = updatedEvent.OwnerId;
            eventToUpdate.Location = updatedEvent.Location;
            eventToUpdate.Status = updatedEvent.Status;
            return;
        }
    }
}