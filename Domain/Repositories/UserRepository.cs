using System;
using System.Threading;
using System.Threading.Tasks;

using Domain.DbContexts;
using Domain.Interfaces;
using Domain.Models.Database;

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
    }
}