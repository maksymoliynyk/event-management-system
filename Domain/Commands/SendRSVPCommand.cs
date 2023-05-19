using System.Threading;
using System.Threading.Tasks;

using Domain.Interfaces;
using Domain.Models.Database;

using MediatR;

namespace Domain.Commands
{
    public class SendRSVPCommand : IRequest<SendRSVPResult>
    {
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

        public SendRSVPCommandHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<SendRSVPResult> Handle(SendRSVPCommand request, CancellationToken cancellationToken)
        {
            UserDTO user = await _repositoryManager.User.GetUserByEmailOrCreateUser(request.UserEmail, cancellationToken);
            EventDTO searchedEvent = await _repositoryManager.Event.GetEventById(request.EventId, cancellationToken);
            RSVPDTO rsvp = await _repositoryManager.RSVP.SendRSVPToUser(searchedEvent, user, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);
            return new SendRSVPResult
            {
                RSVPId = rsvp.Id.ToString()
            };
        }
    }
}