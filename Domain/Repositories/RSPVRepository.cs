using System;
using System.Threading;
using System.Threading.Tasks;

using Domain.DbContexts;
using Domain.Interfaces;
using Domain.Models.Database;

using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories
{
    public class RSPVRepository : IRSPVRepository
    {
        private readonly EventManagementContext _context;

        public RSPVRepository(EventManagementContext context)
        {
            _context = context;
        }

        public async Task<RSPVDTO> GetRSPVById(string id, CancellationToken cancellationToken = default)
        {
            return await _context.RSPVs
                    .Include(t => t.Event)
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id.ToString() == id, cancellationToken);
        }

        public async Task<string> SendRSPVToUser(EventDTO eventDTO, UserDTO userDTO, CancellationToken cancellationToken = default)
        {
            if (eventDTO.OwnerId == userDTO.Id)
            {
                return null;
            }
            RSPVDTO rspvDTO = new()
            {
                Id = Guid.NewGuid(),
                EventId = eventDTO.Id,
                UserId = userDTO.Id,
                Status = 0
            };
            _ = await _context.RSPVs.AddAsync(rspvDTO, cancellationToken);
            return rspvDTO.Id.ToString();
        }
    }
}