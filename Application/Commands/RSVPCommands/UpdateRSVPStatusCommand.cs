using System.Threading;
using System.Threading.Tasks;

using Domain.Aggregates.Events;
using Domain.Enums;
using Domain.Exceptions;

using Infrastructure;

using MediatR;

namespace Application.Commands.RSVPCommands;


public class UpdateRSVPStatusCommand : IRequest<UpdateRSVPStatusResult>
{
    public bool RSVPAccepted { get; init; }
    public string RSVPId { get; init; }
    public string UserId { get; init; }
}

public class UpdateRSVPStatusResult
{
}

public class UpdateRSVPStatusHandler : IRequestHandler<UpdateRSVPStatusCommand, UpdateRSVPStatusResult>
{

    private readonly IUnitOfWork _unitOfWork;

    public UpdateRSVPStatusHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateRSVPStatusResult> Handle(UpdateRSVPStatusCommand request, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.RSVP.IsUserInvited(request.UserId, request.RSVPId, cancellationToken))
        {
            throw new NoAccessException("You have no access for accepting this rvsp");
        }
        RSVPStatus newStatus = request.RSVPAccepted ? RSVPStatus.Accepted : RSVPStatus.Declined;
        await _unitOfWork.RSVP.ChangeRSVPStatus(request.RSVPId, newStatus, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);
        return new UpdateRSVPStatusResult();
    }
}