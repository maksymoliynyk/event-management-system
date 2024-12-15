using Domain.Enums;
using Domain.Interfaces;

namespace Application.Commands.RSVPs;

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

        var newStatus = request.RSVPAccepted ? RSVPStatus.Accepted : RSVPStatus.Declined;
        @event.RespondToRsvp(newStatus, request.UserId);

        await _unitOfWork.SaveAsync(cancellationToken);
        return new UpdateRSVPStatusResult();
    }
}