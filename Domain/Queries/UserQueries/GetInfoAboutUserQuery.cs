using System.Threading;
using System.Threading.Tasks;

using Domain.Interfaces;
using Domain.Models.Database;

using MediatR;

namespace Domain.Queries.UserQueries
{
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
        private readonly IRepositoryManager _repositoryManager;

        public GetInfoAboutUserQueryHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<GetInfoAboutUserResult> Handle(GetInfoAboutUserQuery request, CancellationToken cancellationToken)
        {
            UserDTO result = await _repositoryManager.User.GetUserByUsername(request.UserName, cancellationToken);
            return new GetInfoAboutUserResult
            {
                UserName = result.UserName,
                FullName = result.FullName,
                Email = result.Email
            };
        }
    }
}