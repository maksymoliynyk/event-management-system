using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Aggregates.Events;
using Domain.Exceptions;

using Infrastructure;

using MediatR;

namespace Application.Queries.EventQueries;
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

    private readonly IUnitOfWork _unitOfWork;

    public GetEventByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<GetEventByIdResult> Handle(GetEventByIdQuery request, CancellationToken cancellationToken = default)
    {
        EventDTO searchedEventDTO = await _unitOfWork.Event.GetEventById(request.Id, cancellationToken);
        if (!searchedEventDTO.IsPublic)
        {
            if (request.UserId == null)
            {
                throw new NoAccessException("You do not have access to this event");
            };
            bool hasAccess = searchedEventDTO.OwnerId == request.UserId ||
                            await _unitOfWork.RSVP.IsUserInvited(request.UserId, request.Id, cancellationToken);
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
