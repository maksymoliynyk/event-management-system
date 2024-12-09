using Domain.Entities.Users;

namespace Application.Commands.Auth.Login;

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