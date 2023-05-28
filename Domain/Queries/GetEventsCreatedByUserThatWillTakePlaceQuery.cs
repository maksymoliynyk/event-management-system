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
        public string UserName { get; init; }
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
            UserDTO user = await _repositoryManager.User.GetUserByUsername(request.UserName, cancellationToken);
            IEnumerable<EventDTO> eventsDTO = await _repositoryManager.Event.GetEventsByOwnerByCondition(user.Id,
                                                                                t => (t.Date + t.Duration) > DateTime.Now, cancellationToken);
            return new GetEventsCreatedByUserThatWillTakePlaceResult
            {
                Events = _mapper.Map<IEnumerable<Event>>(eventsDTO)
            };
        }
    }
}