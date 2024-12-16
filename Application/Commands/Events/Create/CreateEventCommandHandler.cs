using System;

using Domain.Aggregates.Events;
using Domain.Interfaces;

namespace Application.Commands.Events;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, CreateEventResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateEventCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateEventResult> Handle(CreateEventCommand request,
        CancellationToken cancellationToken = default)
    {
        var duration = TimeSpan.FromSeconds(request.Duration);
        var newEvent = Event.CreateEvent(request.Title, request.Description, duration, request.Location, request.UserId,
            request.StartDate);
        _unitOfWork.Event.Create(newEvent);
        await _unitOfWork.SaveAsync(cancellationToken);
        return new CreateEventResult(newEvent.Id);
    }
}