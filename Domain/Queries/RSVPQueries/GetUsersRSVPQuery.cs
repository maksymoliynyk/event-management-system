using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Interfaces;

using MediatR;

namespace Domain.Queries.RSVPQueries
{
    public class GetUsersRSVPQuery : IRequest<GetUsersRSVPResult>
    {
        public string UserId { get; init; }
    }

    public class GetUsersRSVPResult
    {
        public IEnumerable<RSVP> RSVPs { get; init; }
    }
    public class GetUsersRSVPHandler : IRequestHandler<GetUsersRSVPQuery, GetUsersRSVPResult>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public GetUsersRSVPHandler(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<GetUsersRSVPResult> Handle(GetUsersRSVPQuery request, CancellationToken cancellationToken)
        {
            return new GetUsersRSVPResult
            {
                RSVPs = _mapper.Map<IEnumerable<RSVP>>(await _repositoryManager.RSVP.GetAllRSVPsForUser(request.UserId, cancellationToken))
            };
        }
    }
}