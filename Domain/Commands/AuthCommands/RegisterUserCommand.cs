using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Domain.Interfaces;
using Domain.Models.Database;

using MediatR;

using Microsoft.AspNetCore.Identity;

namespace Domain.Commands.AuthCommands
{
    public class RegisterUserCommand : IRequest<RegisterUserResult>
    {
        public string Email { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Username { get; init; }
        public string Password { get; init; }
    }

    public class RegisterUserResult
    {
        public bool Succeeded { get; init; }
        public IEnumerable<IdentityError> Errors { get; init; }
    }

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public RegisterUserCommandHandler(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken = default)
        {
            UserDTO userDTO = _mapper.Map<RegisterUserCommand, UserDTO>(request);
            string password = request.Password;
            IdentityResult result = await _repositoryManager.User.RegisterUser(userDTO, password, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);
            return new RegisterUserResult
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors
            };
        }
    }
}