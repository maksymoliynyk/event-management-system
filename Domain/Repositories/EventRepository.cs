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
                throw new ObjectStatusException($"Event status cannot be changed", ObjectStatusError.StatusCannotBeChanged, ObjectType.Event);
            }
            if (eventToChange.Status == (int)eventStatus)
            {
                throw new ObjectStatusException($"Event status is already {eventStatus}", ObjectStatusError.NewStatusCannotBeSameAsOldStatus, ObjectType.Event);
            }
            eventToChange.Status = (int)eventStatus;
            return;
        }

        public async Task<string> CreateEvent(EventDTO newEvent, CancellationToken cancellationToken = default)
        {
            _ = await _context.Events.AddAsync(newEvent, cancellationToken);
            return newEvent.Id;
        }

        public async Task<EventDTO> DeleteEventById(string id, CancellationToken cancellationToken = default)
        {
            EventDTO eventToDelete = await GetEventById(id, cancellationToken);
            _ = _context.Events.Remove(eventToDelete);
            return eventToDelete;
        }

        public Task<IEnumerable<EventDTO>> GetAllEvents(string userId = null, CancellationToken cancellationToken = default)
        {
            if (userId == null)
            {
                return Task.FromResult(_context.Events.Include(x => x.Owner)
                                        .Where(x => x.IsPublic)
                                        .AsEnumerable());
            }
            IEnumerable<string> rsvpsIDs = _context.RSPVs.Where(x => x.UserId == userId)
                                    .Select(x => x.EventId).AsEnumerable();
            return Task.FromResult(_context.Events.Include(x => x.Owner)
                                    .Where(x => rsvpsIDs.Contains(x.Id) || x.OwnerId == userId || x.IsPublic)
                                    .AsEnumerable());
        }

        public Task<IEnumerable<EventDTO>> GetAllEventsWhereUserAreParticipate(string userId, CancellationToken cancellationToken = default)
        {
            IEnumerable<string> rsvps = _context.RSPVs.Where(x => x.UserId == userId)
                                                        .Select(x => x.EventId)
                                                        .AsEnumerable();
            return Task.FromResult(_context.Events.Include(x => x.Owner)
                                    .Where(x => (x.OwnerId == userId) || rsvps.Contains(x.Id))
                                    .AsEnumerable());

        }

        public async Task<EventDTO> GetEventById(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Events
                            .Include(t => t.Owner)
                            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken) ?? throw new ObjectNotFoundException("Event doesn't exist", ObjectNotFoundErrors.Event);
        }

        public Task<IEnumerable<EventDTO>> GetEventsByOwner(string userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_context.Events
                            .Include(t => t.Owner)
                            .Where(t => t.OwnerId == userId)
                            .AsEnumerable());
        }

        public async Task<bool> IsUserOwner(string userId, string eventId, CancellationToken cancellationToken = default)
        {
            return (await GetEventById(eventId, cancellationToken)).OwnerId == userId;
        }


        //? This method is not used in the application, but it can be useful in future

        // public async Task UpdateEvent(EventDTO updatedEvent, CancellationToken cancellationToken = default)
        // {
        //     EventDTO eventToUpdate = await GetEventById(updatedEvent.Id, cancellationToken);
        //     if (eventToUpdate == null)
        //     {
        //         return;
        //     }
        //     eventToUpdate.Id = updatedEvent.Id;
        //     eventToUpdate.Title = updatedEvent.Title;
        //     eventToUpdate.Description = updatedEvent.Description;
        //     eventToUpdate.Date = updatedEvent.Date;
        //     eventToUpdate.Duration = updatedEvent.Duration;
        //     eventToUpdate.OwnerId = updatedEvent.OwnerId;
        //     eventToUpdate.Location = updatedEvent.Location;
        //     eventToUpdate.Status = updatedEvent.Status;
        //     return;
        // }
    }
}