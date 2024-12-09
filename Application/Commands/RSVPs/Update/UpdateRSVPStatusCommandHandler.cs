using System;
using System.Linq;

using Domain.Enums;
using Domain.Interfaces;

namespace Application.Commands.RSVPs.Update;

public class UpdateRSVPStatusCommandHandler : IRequestHandler<UpdateRSVPStatusCommand, UpdateRSVPStatusResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRSVPStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateRSVPStatusResult> Handle(UpdateRSVPStatusCommand request,
        CancellationToken cancellationToken = default)
    {
        var @event = _unitOfWork.Event.GetById(request.EventId);

        if (@event == null)
        {
            throw new NullReferenceException($"Event with id {request.EventId} was not found");
        }

        var newStatus = request.RSVPAccepted ? RSVPStatus.Accepted : RSVPStatus.Declined;

        var rsvp = @event.RSVPs.FirstOrDefault(r => r.UserId == request.UserId);
        if (rsvp == null)
        {
            throw new NullReferenceException($"RSVP with id {request.EventId} was not found");
        }

        rsvp.ChangeStatus(newStatus, request.UserId);

        await _unitOfWork.SaveAsync(cancellationToken);
        return new UpdateRSVPStatusResult();
    }
}