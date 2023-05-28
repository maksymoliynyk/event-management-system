using System.Threading;
using System.Threading.Tasks;

using Contracts.Models.Statuses;

using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Services;

using MediatR;

namespace Domain.Commands
{
    public class CancelEventByIdCommand : IRequest<CancelEventByIdResult>
    {
        public string Id { get; init; }
        public string UserName { get; init; }
    }
    public class CancelEventByIdResult
    {

    }
    public class CancelEventByIdCommandHandler : IRequestHandler<CancelEventByIdCommand, CancelEventByIdResult>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly AccessChecker _accessChecker;

        public CancelEventByIdCommandHandler(IRepositoryManager repositoryManager, AccessChecker accessChecker)
        {
            _repositoryManager = repositoryManager;
            _accessChecker = accessChecker;
        }

        public async Task<CancelEventByIdResult> Handle(CancelEventByIdCommand request, CancellationToken cancellationToken)
        {
            bool hasAccess = await _accessChecker.HasAccessAsOwner(request.Id, request.UserName, cancellationToken);
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