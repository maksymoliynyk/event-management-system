using Domain.Entities.Users;

namespace Application.Commands.Auth.Register;

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