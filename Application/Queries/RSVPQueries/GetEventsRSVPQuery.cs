using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Aggregates.Events;
using Domain.Exceptions;

using Infrastructure;

using MediatR;

namespace Application.Queries.RSVPQueries;

public class GetEventsRSVPQuery : IRequest<GetEventsRSVPResult>
{
    public string EventId { get; init; }
    public string UserId { get; init; }
}

public class GetEventsRSVPResult
{
    public IEnumerable<RSVP> RSVPs { get; init; }
}
public class GetEventsRSVPQueryHandler : IRequestHandler<GetEventsRSVPQuery, GetEventsRSVPResult>
{

    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;
    public GetEventsRSVPQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task<GetEventsRSVPResult> Handle(GetEventsRSVPQuery request, CancellationToken cancellationToken)
    {
        if (!await _unitOfWork.Event.IsUserOwner(request.UserId, request.EventId, cancellationToken))
        {
            throw new NoAccessException("You do not have access to this event");
        }
        IEnumerable<RSVPDTO> rsvps = await _unitOfWork.RSVP.GetAllRSVPsForEvent(request.EventId, cancellationToken);
        return new GetEventsRSVPResult
        {
            RSVPs = _mapper.Map<IEnumerable<RSVP>>(rsvps)
        };
    }
}