using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Domain.Interfaces;
using Domain.Models.Database;

namespace Domain.Services
{
    public class AccessChecker
    {
        private readonly IRepositoryManager _repositoryManager;

        public AccessChecker(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public async Task<bool> HasAccessAsRSVP(string eventId, string userId, CancellationToken cancellationToken = default)
        {
            IEnumerable<RSVPDTO> searchedRSVPs = await _repositoryManager.RSVP.GetAllRSVPsForEvent(eventId, cancellationToken);
            bool result = false;
            foreach (RSVPDTO rsvp in searchedRSVPs)
            {
                if (rsvp.UserId == userId)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public async Task<bool> HasAccessAsOwner(string eventId, string userName, CancellationToken cancellationToken = default)
        {
            UserDTO user = await _repositoryManager.User.GetUserByUsername(userName, cancellationToken);
            EventDTO searchedEvent = await _repositoryManager.Event.GetEventById(eventId, cancellationToken);
            return searchedEvent.OwnerId == user.Id;
        }
    }
}