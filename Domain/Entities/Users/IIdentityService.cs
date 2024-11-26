using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Users;

public interface IIdentityService
{
    // todo: fill parameters from use cases
    Task<string> LoginUserAsync(string email, string password, CancellationToken ct);
    Task<IdentityResult> RegisterUserAsync(string email, string password, string firstName, string lastName, CancellationToken ct);
    Task GenerateResetPasswordTokenAsync(string email);
    Task ResetPasswordAsync();
    Task ChangePasswordAsync();
}