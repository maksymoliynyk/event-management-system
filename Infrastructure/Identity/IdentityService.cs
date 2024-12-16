using System;
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
        throw new NotImplementedException();
    }

    public Task GenerateResetPasswordTokenAsync(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<string> LoginUserAsync(string email, string password, CancellationToken ct)
    {
        var user = await FindByEmail(email);

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            throw new LoginException("Wrong email or password");
        }

        return _tokenProvider.GetToken(user);
    }

    public async Task<IdentityResult> RegisterUserAsync(string email, string password, string firstName,
        string lastName, CancellationToken ct)
    {
        if (await FindByEmail(email) != null)
        {
            throw new ObjectAlreadyExistException(EntitiesErrorType.User);
        }

        var user = new User { UserName = email, Email = email, FirstName = firstName, LastName = lastName };
        var result = await _userManager.CreateAsync(user, password);
        return result;
    }

    public Task ResetPasswordAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var user = await FindByEmail(email);
        if (user == null)
        {
            throw new ObjectNotFoundException(EntitiesErrorType.User);
        }

        return user;
    }

    private async Task<User> FindByEmail(string email) => await _userManager.FindByEmailAsync(email);

    public void Dispose()
    {
        _userManager.Dispose();
    }
}