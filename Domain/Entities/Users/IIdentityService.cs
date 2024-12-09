using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Users;

public interface IIdentityService : IDisposable
{
    // todo: fill parameters from use cases
    Task<string> LoginUserAsync(string email, string password, CancellationToken ct);
    Task<IdentityResult> RegisterUserAsync(string email, string password, string firstName, string lastName, CancellationToken ct);
    Task GenerateResetPasswordTokenAsync(string email);
    Task ResetPasswordAsync();
    Task ChangePasswordAsync();
    Task<User> GetUserByEmail(string email);
}