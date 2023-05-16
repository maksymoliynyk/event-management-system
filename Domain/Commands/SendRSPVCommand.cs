using System.Threading;
using System.Threading.Tasks;

using Domain.Interfaces;
using Domain.Models.Database;

using MediatR;

namespace Domain.Commands
{
    public class SendRSPVCommand : IRequest<SendRSPVResult>
    {
        public string UserEmail { get; init; }
        public string EventId { get; init; }
    }

    public class SendRSPVResult
    {
        public string RSPVId { get; init; }
    }
    public class SendRSPVCommandHandler : IRequestHandler<SendRSPVCommand, SendRSPVResult>
    {
        private readonly IRepositoryManager _repositoryManager;

        public SendRSPVCommandHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<SendRSPVResult> Handle(SendRSPVCommand request, CancellationToken cancellationToken)
        {
            UserDTO user = await _repositoryManager.User.GetUserByEmailOrCreateUser(request.UserEmail, cancellationToken);
            EventDTO searchedEvent = await _repositoryManager.Event.GetEventById(request.EventId, cancellationToken);
            string id = await _repositoryManager.RSPV.SendRSPVToUser(searchedEvent, user, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);
            return new SendRSPVResult
            {
                RSPVId = id
            };
        }
    }
}