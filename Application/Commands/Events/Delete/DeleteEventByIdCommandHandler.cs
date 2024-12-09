using System;

using Domain.Interfaces;

namespace Application.Commands.Events.Delete;

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