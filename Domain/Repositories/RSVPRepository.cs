using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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


        //? This method is not used in the API
        public async Task<RSVPDTO> GetRSVPById(string id, CancellationToken cancellationToken = default)
        {
            return await _context.RSPVs
                    .Include(t => t.Event)
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id.ToString() == id, cancellationToken);
        }

        public async Task<RSVPDTO> SendRSVPToUser(EventDTO eventDTO, UserDTO userDTO, CancellationToken cancellationToken = default)
        {
            if (eventDTO.OwnerId == userDTO.Id)
            {
                throw new RSPVSendException(RSPVSendExceptionError.UserIsOwner, "User is owner of event");
            }
            if (await _context.RSPVs.AnyAsync(t => t.EventId == eventDTO.Id && t.UserId == userDTO.Id, cancellationToken))
            {
                throw new RSPVSendException(RSPVSendExceptionError.UserAlreadyRSVPd, "User has already RSVPd to event");
            }
            RSVPDTO rspvDTO = new()
            {
                Id = Guid.NewGuid(),
                EventId = eventDTO.Id,
                UserId = userDTO.Id,
                Status = 0
            };
            _ = await _context.RSPVs.AddAsync(rspvDTO, cancellationToken);
            return rspvDTO;
        }

        public async Task<IEnumerable<RSVPDTO>> GetAllRSVPsForEvent(string eventId, CancellationToken cancellationToken = default)
        {
            return await _context.RSPVs
                            .Include(t => t.Event)
                            .Include(t => t.User)
                            .Where(t => t.EventId.ToString() == eventId)
                            .ToArrayAsync(cancellationToken);
        }
    }
}