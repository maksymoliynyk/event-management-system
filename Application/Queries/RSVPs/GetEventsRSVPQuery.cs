using System;
using System.Collections.Generic;

using Application.Interfaces.Repositories;
using Application.Models;

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