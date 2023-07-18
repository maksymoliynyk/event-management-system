using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Interfaces;

using MediatR;

namespace Domain.Queries.UserQueries
{
    public class GetAllEventsWhereUserAreParticipateQuery : IRequest<GetAllEventsWhereUserAreParticipateResult>
    {
        public string UserId { get; init; }
    }

    public class GetAllEventsWhereUserAreParticipateResult
    {
        public IEnumerable<Event> Events { get; init; }
    }

    public class GetAllEventsWhereUserAreParticipateHandler : IRequestHandler<GetAllEventsWhereUserAreParticipateQuery, GetAllEventsWhereUserAreParticipateResult>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repositoryManager;
        public GetAllEventsWhereUserAreParticipateHandler(IMapper mapper, IRepositoryManager repositoryManager)
        {
            _mapper = mapper;
            _repositoryManager = repositoryManager;
        }
        public async Task<GetAllEventsWhereUserAreParticipateResult> Handle(GetAllEventsWhereUserAreParticipateQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Event> events = _mapper.Map<IEnumerable<Event>>(await _repositoryManager.Event.GetAllEventsWhereUserAreParticipate(request.UserId, cancellationToken));
            return new GetAllEventsWhereUserAreParticipateResult
            {
                Events = events
            };
        }
    }
}