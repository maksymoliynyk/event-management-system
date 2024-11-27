using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Application.Interfaces;
using Application.Queries.Models;

using MediatR;

namespace Application.Queries.RSVPs;

public sealed record GetEventsRSVPQuery(Guid EventId, Guid OwnerId)
    : IRequest<GetEventsRSVPQueryResult>;

public sealed record GetEventsRSVPQueryResult(IEnumerable<RSVPQueryModel> RSVPs);

public class GetEventsRSVPQueryHandler : IRequestHandler<GetEventsRSVPQuery, GetEventsRSVPQueryResult>
{
    private readonly IEventQueryRepository _repository;

    public GetEventsRSVPQueryHandler(IEventQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetEventsRSVPQueryResult> Handle(GetEventsRSVPQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllInvitesByEvent(request.EventId, request.OwnerId, cancellationToken);
        return new GetEventsRSVPQueryResult(result);
    }
}