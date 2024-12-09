using Domain.Interfaces;

namespace Application.Commands.Events.Cancel;

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

        @event.CancelEvent();
        _unitOfWork.Event.Update(@event);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CancelEventByIdResult();
    }
}