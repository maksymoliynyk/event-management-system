using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;

namespace Application.Commands.Auth;

public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password)
    : IRequest<RegisterUserResult>;

public sealed record RegisterUserResult(bool Succeeded, IEnumerable<IdentityError> Errors);