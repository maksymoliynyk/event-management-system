using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models.Database;

using MediatR;

namespace Domain.Queries.RSVPQueries
{
    public class GetRSVPByIdQuery : IRequest<GetRSVPByIdResult>
    {
        public string RSVPId { get; init; }
        public string UserId { get; init; }
    }

    public class GetRSVPByIdResult
    {
        public RSVP RSVP { get; init; }
    }
    public class GetRSVPByIdHandler : IRequestHandler<GetRSVPByIdQuery, GetRSVPByIdResult>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public GetRSVPByIdHandler(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<GetRSVPByIdResult> Handle(GetRSVPByIdQuery request, CancellationToken cancellationToken)
        {
            RSVPDTO rspvDTO = await _repositoryManager.RSVP.GetRSVPById(request.RSVPId, cancellationToken);
            bool hasNoAccess = !(await _repositoryManager.Event.IsUserOwner(request.UserId, rspvDTO.EventId, cancellationToken)
                    || await _repositoryManager.RSVP.IsUserInvited(request.UserId, rspvDTO.EventId, cancellationToken));
            return hasNoAccess
                ? throw new NoAccessException("You have no access to this RSVP")
                : new GetRSVPByIdResult
                {
                    RSVP = _mapper.Map<RSVP>(rspvDTO)
                };
        }
    }
}