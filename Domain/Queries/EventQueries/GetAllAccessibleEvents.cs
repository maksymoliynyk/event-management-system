using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Interfaces;
using Domain.Models.Database;

using MediatR;

namespace Domain.Queries.EventQueries
{
    public class GetAllAccessibleEventsQuery : IRequest<GetAllAccessibleEventsResult>
    {
        public string UserId { get; init; }
    }

    public class GetAllAccessibleEventsResult
    {
        public IEnumerable<Event> Events { get; init; }
    }
    public class GetAllAccessibleEventsHandler : IRequestHandler<GetAllAccessibleEventsQuery, GetAllAccessibleEventsResult>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repositoryManager;
        public GetAllAccessibleEventsHandler(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }
        public async Task<GetAllAccessibleEventsResult> Handle(GetAllAccessibleEventsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<EventDTO> result = await _repositoryManager.Event.GetAllEvents(request.UserId, cancellationToken);
            return new GetAllAccessibleEventsResult
            {
                Events = _mapper.Map<IEnumerable<Event>>(result)
            };
        }
    }
}