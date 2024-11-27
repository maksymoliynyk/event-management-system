using System;
using System.Threading;
using System.Threading.Tasks;

using Domain.Interfaces;

using MediatR;

namespace Application.Commands.Events;

public sealed record CancelEventByIdCommand(Guid Id, Guid UserId) : IRequest<CancelEventByIdResult>;

public sealed record CancelEventByIdResult;

public class CancelEventByIdCommandHandler : IRequestHandler<CancelEventByIdCommand, CancelEventByIdResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelEventByIdCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelEventByIdResult> Handle(CancelEventByIdCommand request, CancellationToken cancellationToken)
    {
        var @event = _unitOfWork.Event.GetById(request.Id, request.UserId);

        if (@event == null)
        {
            throw new NullReferenceException($"Event with id {request.Id} was not found");
        }

        @event.CancelEvent();
        _unitOfWork.Event.Update(@event);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CancelEventByIdResult();
    }
}