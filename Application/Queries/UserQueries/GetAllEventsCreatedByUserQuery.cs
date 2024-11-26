using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Aggregates.Events;

using Infrastructure;

using MediatR;

namespace Application.Queries.UserQueries;

public class GetAllEventsCreatedByUserQuery : IRequest<GetAllEventsCreatedByUserResult>
{
    public string UserId { get; init; }
}

public class GetAllEventsCreatedByUserResult
{
    public IEnumerable<Event> Events { get; init; }
}
public class GetAllEventsCreatedByUserQueryHandler : IRequestHandler<GetAllEventsCreatedByUserQuery, GetAllEventsCreatedByUserResult>
{

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;
    public GetAllEventsCreatedByUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetAllEventsCreatedByUserResult> Handle(GetAllEventsCreatedByUserQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<EventDTO> eventsDTO = await _unitOfWork.Event.GetEventsByOwner(request.UserId, cancellationToken);
        IEnumerable<Event> events = _mapper.Map<IEnumerable<Event>>(eventsDTO);
        return new GetAllEventsCreatedByUserResult { Events = events };
    }
}