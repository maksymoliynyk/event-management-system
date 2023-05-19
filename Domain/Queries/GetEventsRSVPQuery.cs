using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Interfaces;
using Domain.Models.Database;

using MediatR;

namespace Domain.Queries
{
    public class GetEventsRSVPQuery : IRequest<GetEventsRSVPResult>
    {
        public string EventId { get; init; }
    }

    public class GetEventsRSVPResult
    {
        public IEnumerable<RSVP> RSVPs { get; init; }
    }
    public class GetEventsRSVPQueryHandler : IRequestHandler<GetEventsRSVPQuery, GetEventsRSVPResult>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        public GetEventsRSVPQueryHandler(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<GetEventsRSVPResult> Handle(GetEventsRSVPQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<RSVPDTO> rsvps = await _repositoryManager.RSVP.GetAllRSVPsForEvent(request.EventId, cancellationToken);
            return new GetEventsRSVPResult
            {
                RSVPs = _mapper.Map<IEnumerable<RSVP>>(rsvps)
            };
        }
    }
}