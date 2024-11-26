using System.Threading;
using System.Threading.Tasks;

using Domain.Aggregates.Events;
using Domain.Enums;
using Domain.Exceptions;

using Infrastructure;

using MediatR;

namespace Application.Commands.EventCommands;

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
    
    private readonly IUnitOfWork _unitOfWork;

    public CancelEventByIdCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelEventByIdResult> Handle(CancelEventByIdCommand request, CancellationToken cancellationToken)
    {
        bool hasAccess = await _unitOfWork.Event.IsUserOwner(request.UserId, request.Id, cancellationToken);
        if (!hasAccess)
        {
            throw new NoAccessException("You do not have access to this event");
        }
        await _unitOfWork.Event.ChangeEventStatus(request.Id, EventStatus.Cancelled, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
        return new CancelEventByIdResult();
    }
}
