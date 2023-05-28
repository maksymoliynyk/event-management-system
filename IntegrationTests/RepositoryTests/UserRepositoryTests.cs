using System.Threading.Tasks;

using Domain.DbContexts;
using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Repositories;
using Domain.Services;

using Faker;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Shouldly;

namespace IntegrationTests.RepositoryTests
{
    public class UserRepositoryTests : System.IDisposable
    {
        private readonly UserManager<UserDTO> _userManager;
        private readonly TokenService _tokenService;
        private readonly EventManagementContext _context;
        private readonly IUserRepository _userRepository;

        public UserRepositoryTests()
        {
            _tokenService = new TokenService();
            _context = CreateContext();
            _userManager = new UserManager<UserDTO>(new UserStore<UserDTO>(_context), null, null, null, null, null, null, null, null);
            _userRepository = new UserRepository(_tokenService, _userManager);
        }

        [Fact]
        public async Task RegisterUserShouldReturnIdentityResult()
        {
            // Arrange
            UserDTO userDTO = new()
            {
                Email = Internet.Email(),
                FirstName = Name.First(),
                LastName = Name.Last(),
                UserName = Internet.UserName()
            };
            string password = Lorem.GetFirstWord();

            // Act
            IdentityResult result = await _userRepository.RegisterUser(userDTO, password, default);

            // Assert
            _ = result.ShouldBeOfType<IdentityResult>();
            result.Succeeded.ShouldBeTrue();
            Dispose();
        }

        private static EventManagementContext CreateContext()
        {
            return new EventManagementContext(new DbContextOptionsBuilder<EventManagementContext>()
                            .UseNpgsql("Host=localhost;Database=EventManagement;Username=postgres;Password=db_learn")
                            .Options);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}