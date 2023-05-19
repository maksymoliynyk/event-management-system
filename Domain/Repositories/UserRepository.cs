using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Domain.DbContexts;
using Domain.Interfaces;
using Domain.Models.Database;

using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EventManagementContext _context;

        public UserRepository(EventManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventDTO>> GetAllEventsCreatedByUser(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Events
                        .Include(t => t.Owner)
                        .Where(t => t.OwnerId.ToString() == id)
                        .ToArrayAsync(cancellationToken);
        }
        public async Task<IEnumerable<EventDTO>> GetEventsCreatedByUserByCondition(string id, Func<EventDTO, bool> condition, CancellationToken cancellationToken = default)
        {
            IEnumerable<EventDTO> users = await GetAllEventsCreatedByUser(id, cancellationToken);
            return users.Where(condition).ToArray();
        }

        public async Task<UserDTO> GetUserByEmailOrCreateUser(string email, CancellationToken cancellationToken = default)
        {
            UserDTO user = await _context.Users.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
            if (user != null)
            {
                return user;
            }

            user = new UserDTO
            {
                Id = Guid.NewGuid(),
                Email = email
            };

            _ = await _context.Users.AddAsync(user, cancellationToken);
            return user;
        }

        public async Task<UserDTO> GetUserById(string id, CancellationToken cancellationToken = default)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Id.ToString() == id, cancellationToken);
        }
    }
}