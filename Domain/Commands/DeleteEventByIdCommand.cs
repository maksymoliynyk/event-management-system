using System.Threading;
using System.Threading.Tasks;

using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Services;

using MediatR;

namespace Domain.Commands
{
    public class DeleteEventByIdCommand : IRequest<DeleteEventByIdResult>
    {
        public string Id { get; init; }
        public string UserName { get; init; }
    }
    public class DeleteEventByIdResult
    {

    }
    public class DeleteEventByIdCommandHandler : IRequestHandler<DeleteEventByIdCommand, DeleteEventByIdResult>
    {

        private readonly IRepositoryManager _repositoryManager;
        private readonly AccessChecker _accessChecker;

        public DeleteEventByIdCommandHandler(IRepositoryManager repositoryManager, AccessChecker accessChecker)
        {
            _repositoryManager = repositoryManager;
            _accessChecker = accessChecker;
        }

        public async Task<DeleteEventByIdResult> Handle(DeleteEventByIdCommand request, CancellationToken cancellationToken)
        {
            bool hasAccess = await _accessChecker.HasAccessAsOwner(request.Id, request.UserName, cancellationToken);
            if (!hasAccess)
            {
                throw new NoAccessException("You do not have access to this event");
            }
            _ = await _repositoryManager.Event.DeleteEventById(request.Id, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);
            return new DeleteEventByIdResult();
        }
    }
}