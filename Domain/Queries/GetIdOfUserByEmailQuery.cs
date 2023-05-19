using System.Threading;
using System.Threading.Tasks;

using Domain.Interfaces;
using Domain.Models.Database;

using MediatR;

namespace Domain.Queries
{
    public class GetIdOfUserByEmailQuery : IRequest<GetIdOfUserByEmailResult>
    {
        public string Email { get; init; }
    }

    public class GetIdOfUserByEmailResult
    {
        public string Id { get; init; }
    }
    public class GetIdOfUserByEmailQueryHandler : IRequestHandler<GetIdOfUserByEmailQuery, GetIdOfUserByEmailResult>
    {
        private readonly IRepositoryManager _repositoryManager;

        public GetIdOfUserByEmailQueryHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<GetIdOfUserByEmailResult> Handle(GetIdOfUserByEmailQuery request, CancellationToken cancellationToken)
        {
            UserDTO result = await _repositoryManager.User.GetUserByEmailOrCreateUser(request.Email, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);
            return new GetIdOfUserByEmailResult
            {
                Id = result.Id.ToString()
            };
        }
    }
}