using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models.Database;

using MediatR;

namespace Domain.Queries.EventQueries
{
    public class GetEventByIdQuery : IRequest<GetEventByIdResult>
    {
        public string Id { get; init; }
        public string UserId { get; init; }
    }

    public class GetEventByIdResult
    {
        public Event SearchedEvent { get; init; }

    }
    public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, GetEventByIdResult>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repositoryManager;

        public GetEventByIdQueryHandler(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<GetEventByIdResult> Handle(GetEventByIdQuery request, CancellationToken cancellationToken = default)
        {
            EventDTO searchedEventDTO = await _repositoryManager.Event.GetEventById(request.Id, cancellationToken);
            if (!searchedEventDTO.IsPublic)
            {
                if (request.UserId == null)
                {
                    throw new NoAccessException("You do not have access to this event");
                };
                bool hasAccess = searchedEventDTO.OwnerId == request.UserId ||
                                await _repositoryManager.RSVP.IsUserInvited(request.UserId, request.Id, cancellationToken);
                if (!hasAccess)
                {
                    throw new NoAccessException("You do not have access to this event");
                }
            }
            Event searchedEvent = _mapper.Map<Event>(searchedEventDTO);
            return new GetEventByIdResult
            {
                SearchedEvent = searchedEvent
            };
        }
    }
}
