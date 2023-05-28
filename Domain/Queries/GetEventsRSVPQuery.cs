using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models.Database;
using Domain.Services;

using MediatR;

namespace Domain.Queries
{
    public class GetEventsRSVPQuery : IRequest<GetEventsRSVPResult>
    {
        public string EventId { get; init; }
        public string UserName { get; init; }
    }

    public class GetEventsRSVPResult
    {
        public IEnumerable<RSVP> RSVPs { get; init; }
    }
    public class GetEventsRSVPQueryHandler : IRequestHandler<GetEventsRSVPQuery, GetEventsRSVPResult>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly AccessChecker _accessChecker;
        public GetEventsRSVPQueryHandler(IRepositoryManager repositoryManager, IMapper mapper, AccessChecker accessChecker)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _accessChecker = accessChecker;
        }

        public async Task<GetEventsRSVPResult> Handle(GetEventsRSVPQuery request, CancellationToken cancellationToken)
        {
            if (!await _accessChecker.HasAccessAsOwner(request.EventId, request.UserName, cancellationToken))
            {
                throw new NoAccessException("You do not have access to this event");
            }
            IEnumerable<RSVPDTO> rsvps = await _repositoryManager.RSVP.GetAllRSVPsForEvent(request.EventId, cancellationToken);
            return new GetEventsRSVPResult
            {
                RSVPs = _mapper.Map<IEnumerable<RSVP>>(rsvps)
            };
        }
    }
}