using System.Threading;
using System.Threading.Tasks;

using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Services;

using MediatR;

namespace Domain.Commands
{
    public class SendRSVPCommand : IRequest<SendRSVPResult>
    {
        public string UserName { get; init; }
        public string UserEmail { get; init; }
        public string EventId { get; init; }
    }

    public class SendRSVPResult
    {
        public string RSVPId { get; init; }
    }
    public class SendRSVPCommandHandler : IRequestHandler<SendRSVPCommand, SendRSVPResult>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly AccessChecker _accessChecker;

        public SendRSVPCommandHandler(IRepositoryManager repositoryManager, AccessChecker accessChecker)
        {
            _repositoryManager = repositoryManager;
            _accessChecker = accessChecker;
        }

        public async Task<SendRSVPResult> Handle(SendRSVPCommand request, CancellationToken cancellationToken)
        {
            EventDTO searchedEvent = await _repositoryManager.Event.GetEventById(request.EventId, cancellationToken);
            if (!searchedEvent.IsPublic)
            {
                if (!await _accessChecker.HasAccessAsOwner(request.EventId, request.UserName, cancellationToken))
                {
                    throw new NoAccessException("You do not have access to this event");
                }
            }
            UserDTO user = await _repositoryManager.User.GetUserByEmail(request.UserEmail, cancellationToken);
            RSVPDTO rsvp = await _repositoryManager.RSVP.SendRSVPToUser(searchedEvent, user, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);
            return new SendRSVPResult
            {
                RSVPId = rsvp.Id.ToString()
            };
        }
    }
}