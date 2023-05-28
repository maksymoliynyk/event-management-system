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
    public class GetEventByIdQuery : IRequest<GetEventByIdResult>
    {
        public string Id { get; init; }
        public string UserName { get; init; }
    }

    public class GetEventByIdResult
    {
        public Event SearchedEvent { get; init; }

    }
    public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, GetEventByIdResult>
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryManager _repositoryManager;
        private readonly AccessChecker _accessChecker;

        public GetEventByIdQueryHandler(IRepositoryManager repositoryManager, IMapper mapper, AccessChecker accessChecker)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _accessChecker = accessChecker;
        }

        public async Task<GetEventByIdResult> Handle(GetEventByIdQuery request, CancellationToken cancellationToken = default)
        {
            EventDTO searchedEventDTO = await _repositoryManager.Event.GetEventById(request.Id, cancellationToken);
            if (!searchedEventDTO.IsPublic)
            {
                if (request.UserName == null)
                {
                    throw new NoAccessException("You do not have access to this event");
                };
                UserDTO user = await _repositoryManager.User.GetUserByUsername(request.UserName, cancellationToken);
                bool hasAccess = (searchedEventDTO.OwnerId == user.Id) ||
                                await _accessChecker.HasAccessAsRSVP(request.Id, user.Id, cancellationToken);
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
