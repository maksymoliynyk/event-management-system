using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Domain.Entities.Users;

using MediatR;

using Microsoft.AspNetCore.Identity;

namespace Application.Commands.Auth;

public sealed record RegisterUserCommand(
    string Email,
    string FirstName,
    string LastName,
    string Password)
    : IRequest<RegisterUserResult>;

public sealed record RegisterUserResult(bool Succeeded, IEnumerable<IdentityError> Errors);

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IIdentityService _identityService;

    public RegisterUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request,
        CancellationToken cancellationToken = default)
    {
        var result = await _identityService.RegisterUserAsync(request.Email, request.Password,
            request.FirstName, request.LastName, cancellationToken);

        return new RegisterUserResult(result.Succeeded, result.Errors);
    }
}