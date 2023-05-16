using System;
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
    public class GetEventsCreatedByUserThatWillTakePlaceQuery : IRequest<GetEventsCreatedByUserThatWillTakePlaceResult>
    {
        public string Id { get; init; }
    }

    public class GetEventsCreatedByUserThatWillTakePlaceResult
    {
        public IEnumerable<Event> Events { get; init; }
    }
    public class GetEventsCreatedByUserThatWillTakePlaceQueryHandler : IRequestHandler<GetEventsCreatedByUserThatWillTakePlaceQuery, GetEventsCreatedByUserThatWillTakePlaceResult>
    {
        private readonly IRepositoryManager _repositoryManager;

        private readonly IMapper _mapper;
        public GetEventsCreatedByUserThatWillTakePlaceQueryHandler(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<GetEventsCreatedByUserThatWillTakePlaceResult> Handle(GetEventsCreatedByUserThatWillTakePlaceQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<EventDTO> eventsDTO = await _repositoryManager.User.GetEventsCreatedByUserByCondition(request.Id,
                t => (t.Status != 2) && (t.Status != 1) && (DateTimeOffset.Now < t.Date.Add(t.Duration))
                , cancellationToken);
            return new GetEventsCreatedByUserThatWillTakePlaceResult
            {
                Events = _mapper.Map<IEnumerable<Event>>(eventsDTO)
            };
        }
    }
}