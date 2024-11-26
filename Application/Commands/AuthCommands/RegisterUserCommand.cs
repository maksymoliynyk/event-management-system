using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Domain.Models.Database;

using Infrastructure;

using MediatR;

using Microsoft.AspNetCore.Identity;

namespace Application.Commands.AuthCommands;

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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RegisterUserCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken = default)
    {
        UserDTO userDTO = _mapper.Map<RegisterUserCommand, UserDTO>(request);
        string password = request.Password;
        IdentityResult result = await _unitOfWork.User.RegisterUser(userDTO, password, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
        return new RegisterUserResult
        {
            Succeeded = result.Succeeded,
            Errors = result.Errors
        };
    }
}