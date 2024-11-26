using System.Threading;
using System.Threading.Tasks;

using Domain.Models;

using Infrastructure;

using MediatR;

namespace Application.Commands.AuthCommands;

public class LoginUserCommand : IRequest<LoginUserResult>
{
    public string Email { get; init; }
    public string Password { get; init; }
}

public class LoginUserResult
{
    public TokenModel Token { get; init; }

}

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public LoginUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        TokenModel result = await _unitOfWork.User.LoginUser(request.Email, request.Password, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
        return new LoginUserResult { Token = result };
    }
}