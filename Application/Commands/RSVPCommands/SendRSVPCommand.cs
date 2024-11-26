using System.Threading;
using System.Threading.Tasks;

using Domain.Aggregates.Events;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Models.Database;

using Infrastructure;

using MediatR;

namespace Application.Commands.RSVPCommands;

public class SendRSVPCommand : IRequest<SendRSVPResult>
{
    public string UserOwnerId { get; init; }
    public string UserInviteEmail { get; init; }
    public string EventId { get; init; }
}

public class SendRSVPResult
{
    public string RSVPId { get; init; }
}
public class SendRSVPCommandHandler : IRequestHandler<SendRSVPCommand, SendRSVPResult>
{

    private readonly IUnitOfWork _unitOfWork;

    public SendRSVPCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SendRSVPResult> Handle(SendRSVPCommand request, CancellationToken cancellationToken)
    {
        UserDTO user = await _unitOfWork.User.GetUserByEmail(request.UserInviteEmail, cancellationToken);
        EventDTO searchedEvent = await _unitOfWork.Event.GetEventById(request.EventId, cancellationToken);
        if (!searchedEvent.IsPublic)
        {
            if (!(searchedEvent.OwnerId == request.UserOwnerId))
            {
                throw new NoAccessException("You have no access for inviting users");
            }
        }
        if (await _unitOfWork.Event.IsUserOwner(user.Id, searchedEvent.Id, cancellationToken))
        {
            throw new RSPVSendException(RSPVSendExceptionError.UserIsOwner, "User is owner of event");
        }
        if (await _unitOfWork.RSVP.IsUserInvited(user.Id, searchedEvent.Id, cancellationToken))
        {
            throw new RSPVSendException(RSPVSendExceptionError.UserAlreadyRSVPd, "User has already invited to event");
        }
        RSVPDTO rsvp = await _unitOfWork.RSVP.SendRSVPToUser(request.EventId, user.Id, cancellationToken);
        if (searchedEvent.IsPublic)
        {
            await _unitOfWork.RSVP.ChangeRSVPStatus(rsvp.Id, RSVPStatus.Accepted, cancellationToken);
        }
        await _unitOfWork.SaveAsync(cancellationToken);
        return new SendRSVPResult
        {
            RSVPId = rsvp.Id
        };
    }
}