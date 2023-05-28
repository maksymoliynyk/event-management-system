using System.Threading;
using System.Threading.Tasks;

using Domain.DbContexts;
using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Services;

using Microsoft.AspNetCore.Identity;

namespace Domain.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly EventManagementContext _context;
        private readonly TokenService _tokenService;
        private readonly UserManager<UserDTO> _userManager;


        private IUserRepository _userRepository;
        private IEventRepository _eventRepository;
        private IRSVPRepository _rsvpRepository;
        public RepositoryManager(EventManagementContext context, TokenService tokenService, UserManager<UserDTO> userManager)
        {
            _context = context;
            _tokenService = tokenService;
            _userManager = userManager;
        }
        public IUserRepository User
        {
            get
            {
                _userRepository ??= new UserRepository(_tokenService, _userManager);
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
        public IRSVPRepository RSVP
        {
            get
            {
                _rsvpRepository ??= new RSVPRepository(_context);
                return _rsvpRepository;
            }
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            _ = await _context.SaveChangesAsync(cancellationToken);
        }
    }
}