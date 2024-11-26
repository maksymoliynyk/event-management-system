using System.Threading;
using System.Threading.Tasks;

using Domain.Models.Database;

using Infrastructure;

using MediatR;

namespace Application.Queries.UserQueries;

public class GetInfoAboutUserQuery : IRequest<GetInfoAboutUserResult>
{
    public string UserName { get; init; }
}

public class GetInfoAboutUserResult
{
    public string UserName { get; init; }
    public string FullName { get; init; }
    public string Email { get; init; }
}
public class GetInfoAboutUserQueryHandler : IRequestHandler<GetInfoAboutUserQuery, GetInfoAboutUserResult>
{

    private readonly IUnitOfWork _unitOfWork;

    public GetInfoAboutUserQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<GetInfoAboutUserResult> Handle(GetInfoAboutUserQuery request, CancellationToken cancellationToken)
    {
        UserDTO result = await _unitOfWork.User.GetUserByUsername(request.UserName, cancellationToken);
        return new GetInfoAboutUserResult
        {
            UserName = result.UserName,
            FullName = result.FullName,
            Email = result.Email
        };
    }
}