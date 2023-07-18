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
    public class RSVPRepository : IRSVPRepository
    {
        private readonly EventManagementContext _context;

        public RSVPRepository(EventManagementContext context)
        {
            _context = context;
        }

        public async Task<RSVPDTO> GetRSVPById(string id, CancellationToken cancellationToken = default)
        {
            return await _context.RSPVs
                    .Include(t => t.Event)
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id == id, cancellationToken) ?? throw new ObjectNotFoundException("RSVP doesn't exist", ObjectNotFoundErrors.RSVP); ;
        }

        public Task<IEnumerable<RSVPDTO>> GetAllRSVPsForEvent(string eventId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_context.RSPVs
                            .Include(t => t.Event)
                            .Include(t => t.User)
                            .Where(t => t.EventId == eventId)
                            .AsEnumerable());
        }

        public async Task<RSVPDTO> SendRSVPToUser(string eventId, string userId, CancellationToken cancellationToken = default)
        {
            RSVPDTO rspvDTO = new()
            {
                Id = Guid.NewGuid().ToString(),
                EventId = eventId,
                UserId = userId,
                Status = 0
            };

            _ = await _context.RSPVs.AddAsync(rspvDTO, cancellationToken);
            return rspvDTO;
        }

        public async Task ChangeRSVPStatus(string id, RSVPStatus rsvpStatus, CancellationToken cancellationToken = default)
        {
            RSVPDTO rsvpToChange = await _context.RSPVs.FirstAsync(x => x.Id == id, cancellationToken);
            if (rsvpToChange.Status is 1 or 2)
            {
                throw new ObjectStatusException($"RSVP status cannot be changed", ObjectStatusError.StatusCannotBeChanged, ObjectType.RSVP);
            }
            if (rsvpToChange.Status == (int)rsvpStatus)
            {
                throw new ObjectStatusException($"RSVP status is already {rsvpStatus}", ObjectStatusError.NewStatusCannotBeSameAsOldStatus, ObjectType.RSVP);
            }
            rsvpToChange.Status = (int)rsvpStatus;
        }

        public Task<IEnumerable<RSVPDTO>> GetAllRSVPsForUser(string userId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_context.RSPVs
                            .Include(t => t.Event)
                            .Include(t => t.User)
                            .Where(t => t.UserId == userId)
                            .AsEnumerable());
        }
        public async Task<bool> IsUserInvited(string userId, string eventId, CancellationToken cancellationToken = default)
        {
            return await _context.RSPVs.FirstOrDefaultAsync(x => (x.EventId == eventId) && (x.UserId == userId), cancellationToken) != null;
        }
    }
}