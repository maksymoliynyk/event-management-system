using System;
using System.Threading;
using System.Threading.Tasks;

using Domain.Aggregates.Events;
using Domain.Interfaces;

using MediatR;

namespace Application.Commands.Events;

public sealed record CreateEventCommand(
    string Title,
    string Description,
    DateTime StartDate,
    long Duration,
    string Location,
    Guid UserId)
    : IRequest<CreateEventResult>;

public sealed record CreateEventResult(Guid Id);

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