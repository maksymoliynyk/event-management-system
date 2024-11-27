using System;
using System.Threading;
using System.Threading.Tasks;

using Domain.Interfaces;

using MediatR;

namespace Application.Commands.Events;

public sealed record DeleteEventByIdCommand(Guid Id, Guid UserId) : IRequest<DeleteEventByIdResult>;

public sealed record DeleteEventByIdResult;

public class DeleteEventByIdCommandHandler : IRequestHandler<DeleteEventByIdCommand, DeleteEventByIdResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEventByIdCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteEventByIdResult> Handle(DeleteEventByIdCommand request, CancellationToken cancellationToken)
    {
        var @event = _unitOfWork.Event.GetById(request.Id, request.UserId);

        if (@event == null)
        {
            throw new NullReferenceException($"Event with id {request.Id} was not found");
        }

        _unitOfWork.Event.Delete(@event);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new DeleteEventByIdResult();
    }
}