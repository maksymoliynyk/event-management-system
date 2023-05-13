using System.Threading;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRepositoryManager
    {
        IEventRepository Event { get; }
        IUserRepository User { get; }
        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}