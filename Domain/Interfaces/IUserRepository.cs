using System.Threading;
using System.Threading.Tasks;

using Domain.Models.Database;

using Microsoft.AspNetCore.Identity;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        public Task<IdentityResult> RegisterUser(UserDTO userDTO, string password, CancellationToken cancellationToken = default);
        public Task<UserDTO> GetUserByEmail(string email, CancellationToken cancellationToken = default);
        public Task<UserDTO> GetUserByUsername(string username, CancellationToken cancellationToken = default);
        public Task<string> LoginUser(string email, string password, CancellationToken cancellationToken = default);
    }
}