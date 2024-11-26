using System.Threading;
using System.Threading.Tasks;

using Domain.Exceptions;

using Infrastructure;

using MediatR;

namespace Application.Commands.EventCommands;

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
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEventByIdCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteEventByIdResult> Handle(DeleteEventByIdCommand request, CancellationToken cancellationToken)
    {
        bool hasAccess = await _unitOfWork.Event.IsUserOwner(request.UserId, request.Id, cancellationToken);
        if (!hasAccess)
        {
            throw new NoAccessException("You do not have access to this event");
        }
        _ = await _unitOfWork.Event.DeleteEventById(request.Id, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
        return new DeleteEventByIdResult();
    }
}
