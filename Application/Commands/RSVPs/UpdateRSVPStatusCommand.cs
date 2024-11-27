using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Domain.Enums;
using Domain.Interfaces;

using MediatR;

namespace Application.Commands.RSVPs;

public sealed record UpdateRSVPStatusCommand(bool RSVPAccepted, Guid EventId, Guid UserId)
    : IRequest<UpdateRSVPStatusResult>;

public sealed record UpdateRSVPStatusResult;

public class UpdateRSVPStatusHandler : IRequestHandler<UpdateRSVPStatusCommand, UpdateRSVPStatusResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRSVPStatusHandler(IUnitOfWork unitOfWork)
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