using System.Threading;
using System.Threading.Tasks;

using Contracts.Models.Statuses;

using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models.Database;

using MediatR;

namespace Domain.Commands.RSVPCommands
{
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
        private readonly IRepositoryManager _repositoryManager;

        public SendRSVPCommandHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<SendRSVPResult> Handle(SendRSVPCommand request, CancellationToken cancellationToken)
        {
            UserDTO user = await _repositoryManager.User.GetUserByEmail(request.UserInviteEmail, cancellationToken);
            EventDTO searchedEvent = await _repositoryManager.Event.GetEventById(request.EventId, cancellationToken);
            if (!searchedEvent.IsPublic)
            {
                if (!(searchedEvent.OwnerId == request.UserOwnerId))
                {
                    throw new NoAccessException("You have no access for inviting users");
                }
            }
            if (await _repositoryManager.Event.IsUserOwner(user.Id, searchedEvent.Id, cancellationToken))
            {
                throw new RSPVSendException(RSPVSendExceptionError.UserIsOwner, "User is owner of event");
            }
            if (await _repositoryManager.RSVP.IsUserInvited(user.Id, searchedEvent.Id, cancellationToken))
            {
                throw new RSPVSendException(RSPVSendExceptionError.UserAlreadyRSVPd, "User has already invited to event");
            }
            RSVPDTO rsvp = await _repositoryManager.RSVP.SendRSVPToUser(request.EventId, user.Id, cancellationToken);
            if (searchedEvent.IsPublic)
            {
                await _repositoryManager.RSVP.ChangeRSVPStatus(rsvp.Id, RSVPStatus.Accepted, cancellationToken);
            }
            await _repositoryManager.SaveAsync(cancellationToken);
            return new SendRSVPResult
            {
                RSVPId = rsvp.Id
            };
        }
    }
}