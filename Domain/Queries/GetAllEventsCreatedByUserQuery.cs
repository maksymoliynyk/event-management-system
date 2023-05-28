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
    public class GetAllEventsCreatedByUserQuery : IRequest<GetAllEventsCreatedByUserResult>
    {
        public string UserName { get; init; }
    }

    public class GetAllEventsCreatedByUserResult
    {
        public IEnumerable<Event> Events { get; init; }
    }
    public class GetAllEventsCreatedByUserQueryHandler : IRequestHandler<GetAllEventsCreatedByUserQuery, GetAllEventsCreatedByUserResult>
    {
        private readonly IRepositoryManager _repositoryManager;

        private readonly IMapper _mapper;
        public GetAllEventsCreatedByUserQueryHandler(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<GetAllEventsCreatedByUserResult> Handle(GetAllEventsCreatedByUserQuery request, CancellationToken cancellationToken)
        {
            UserDTO user = await _repositoryManager.User.GetUserByUsername(request.UserName, cancellationToken);
            IEnumerable<EventDTO> eventsDTO = await _repositoryManager.Event.GetEventsByOwner(user.Id, cancellationToken);
            IEnumerable<Event> events = _mapper.Map<IEnumerable<Event>>(eventsDTO);
            return new GetAllEventsCreatedByUserResult { Events = events };
        }
    }
}