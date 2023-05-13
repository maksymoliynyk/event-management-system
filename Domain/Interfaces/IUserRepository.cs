using System.Threading;
using System.Threading.Tasks;

using Domain.Models.Database;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<UserDTO> GetUserByEmailOrCreateUser(string email, CancellationToken cancellationToken = default);
    }
}