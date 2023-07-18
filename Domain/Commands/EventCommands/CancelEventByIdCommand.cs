using System.Threading;
using System.Threading.Tasks;

using Contracts.Models.Statuses;

using Domain.Exceptions;
using Domain.Interfaces;

using MediatR;

namespace Domain.Commands.EventCommands
{
    public class CancelEventByIdCommand : IRequest<CancelEventByIdResult>
    {
        public string Id { get; init; }
        public string UserId { get; init; }
    }
    public class CancelEventByIdResult
    {

    }
    public class CancelEventByIdCommandHandler : IRequestHandler<CancelEventByIdCommand, CancelEventByIdResult>
    {
        private readonly IRepositoryManager _repositoryManager;

        public CancelEventByIdCommandHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<CancelEventByIdResult> Handle(CancelEventByIdCommand request, CancellationToken cancellationToken)
        {
            bool hasAccess = await _repositoryManager.Event.IsUserOwner(request.UserId, request.Id, cancellationToken);
            if (!hasAccess)
            {
                throw new NoAccessException("You do not have access to this event");
            }
            await _repositoryManager.Event.ChangeEventStatus(request.Id, EventStatus.Cancelled, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);
            return new CancelEventByIdResult();
        }
    }
}