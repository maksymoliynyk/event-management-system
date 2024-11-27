using System.Threading;
using System.Threading.Tasks;

using Domain.Entities.Users;

using MediatR;

namespace Application.Commands.Auth;

public sealed record LoginUserCommand(string Email, string Password) : IRequest<LoginUserResult>;

public sealed record LoginUserResult(string Token);

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    private readonly IIdentityService _identityService;

    public LoginUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.LoginUserAsync(request.Email, request.Password, cancellationToken);
        return new LoginUserResult(result);
    }
}