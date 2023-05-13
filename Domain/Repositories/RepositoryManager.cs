using System.Threading;
using System.Threading.Tasks;

using Domain.DbContexts;
using Domain.Interfaces;

namespace Domain.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly EventManagementContext _context;


        private IUserRepository _userRepository;
        private IEventRepository _eventRepository;
        public RepositoryManager(EventManagementContext context)
        {
            _context = context;
        }
        public IUserRepository User
        {
            get
            {
                _userRepository ??= new UserRepository(_context);
                return _userRepository;
            }
        }

        public IEventRepository Event
        {
            get
            {
                _eventRepository ??= new EventRepository(_context);
                return _eventRepository;
            }
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            _ = await _context.SaveChangesAsync(cancellationToken);
        }
    }
}