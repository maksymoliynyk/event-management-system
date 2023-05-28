using System.Threading;
using System.Threading.Tasks;

using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Services;

using Microsoft.AspNetCore.Identity;

namespace Domain.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<UserDTO> _userManager;
        private readonly TokenService _tokenService;

        public UserRepository(TokenService tokenService, UserManager<UserDTO> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<UserDTO> GetUserByEmail(string email, CancellationToken cancellationToken = default)
        {
            UserDTO result = await _userManager.FindByEmailAsync(email);
            return result;
        }


        public async Task<IdentityResult> RegisterUser(UserDTO userDTO, string password, CancellationToken cancellationToken = default)
        {
            IdentityResult result = await _userManager.CreateAsync(userDTO, password);
            return result;
        }
        public async Task<string> LoginUser(string email, string password, CancellationToken cancellationToken = default)
        {
            UserDTO user = await _userManager.FindByEmailAsync(email) ?? throw new LoginException(LoginExceptionError.UserNotFound, "User not found");
            bool result = await _userManager.CheckPasswordAsync(user, password);
            return result ? _tokenService.CreateToken(user) :
                throw new LoginException(LoginExceptionError.PasswordIncorrect, "Password incorrect");
        }

        public async Task<UserDTO> GetUserByUsername(string username, CancellationToken cancellationToken = default)
        {
            UserDTO result = await _userManager.FindByNameAsync(username);
            return result;
        }
    }
}