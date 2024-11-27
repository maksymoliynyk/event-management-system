using System.Threading;
using System.Threading.Tasks;

using Domain.Entities.Users;
using Domain.Exceptions;

using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenProvider _tokenProvider;

    public IdentityService(UserManager<User> userManager, ITokenProvider tokenProvider)
    {
        _userManager = userManager;
        _tokenProvider = tokenProvider;
    }

    public Task ChangePasswordAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task GenerateResetPasswordTokenAsync(string email)
    {
        throw new System.NotImplementedException();
    }

    public async Task<string> LoginUserAsync(string email, string password, CancellationToken ct)
    {
        var user = await GetUserByEmail(email);
        var result = await _userManager.CheckPasswordAsync(user, password);
        return result ?
            _tokenProvider.GetToken(user) :
            throw new LoginException(LoginExceptionError.PasswordIncorrect, "Password incorrect");
    }

    public async Task<IdentityResult> RegisterUserAsync(string email, string password, string firstName, string lastName, CancellationToken ct)
    {
        if (await GetUserByEmail(email) != null)
        {
            throw new System.Exception("User exist");
        }
        
        var user = new User
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };
        var result = await _userManager.CreateAsync(user, password);
        return result;
    }

    public Task ResetPasswordAsync()
    {
        throw new System.NotImplementedException();
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }
}