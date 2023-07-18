using System.Threading;
using System.Threading.Tasks;

using Domain.Exceptions;
using Domain.Interfaces;

using MediatR;

namespace Domain.Commands.EventCommands
{
    public class DeleteEventByIdCommand : IRequest<DeleteEventByIdResult>
    {
        public string Id { get; init; }
        public string UserId { get; init; }
    }
    public class DeleteEventByIdResult
    {

    }
    public class DeleteEventByIdCommandHandler : IRequestHandler<DeleteEventByIdCommand, DeleteEventByIdResult>
    {

        private readonly IRepositoryManager _repositoryManager;

        public DeleteEventByIdCommandHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<DeleteEventByIdResult> Handle(DeleteEventByIdCommand request, CancellationToken cancellationToken)
        {
            bool hasAccess = await _repositoryManager.Event.IsUserOwner(request.UserId, request.Id, cancellationToken);
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