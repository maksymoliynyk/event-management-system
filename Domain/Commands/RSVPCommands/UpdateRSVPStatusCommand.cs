using System.Threading;
using System.Threading.Tasks;

using Contracts.Models.Statuses;

using Domain.Exceptions;
using Domain.Interfaces;

using MediatR;

namespace Domain.Commands.RSVPCommands
{
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
        private readonly IRepositoryManager _repositoryManager;

        public UpdateRSVPStatusHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<UpdateRSVPStatusResult> Handle(UpdateRSVPStatusCommand request, CancellationToken cancellationToken = default)
        {
            if (!await _repositoryManager.RSVP.IsUserInvited(request.UserId, request.RSVPId, cancellationToken))
            {
                throw new NoAccessException("You have no access for accepting this rvsp");
            }
            RSVPStatus newStatus = request.RSVPAccepted ? RSVPStatus.Accepted : RSVPStatus.Declined;
            await _repositoryManager.RSVP.ChangeRSVPStatus(request.RSVPId, newStatus, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);
            return new UpdateRSVPStatusResult();
        }
    }
}