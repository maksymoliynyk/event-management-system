using System.Threading;
using System.Threading.Tasks;

using Domain.Exceptions;
using Domain.Interfaces;

using MediatR;

namespace Domain.Commands.AuthCommands
{
    public class LoginUserCommand : IRequest<LoginUserResult>
    {
        public string Email { get; init; }
        public string Password { get; init; }
    }

    public class LoginUserResult
    {
        public string Token { get; init; }
        public LoginException Error { get; init; }

    }
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResult>
    {
        private readonly IRepositoryManager _repositoryManager;

        public LoginUserCommandHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string result = await _repositoryManager.User.LoginUser(request.Email, request.Password, cancellationToken);
                await _repositoryManager.SaveAsync(cancellationToken);
                return new LoginUserResult { Token = result };
            }
            catch (LoginException e)
            {
                return new LoginUserResult { Error = e };
            }

        }
    }
}