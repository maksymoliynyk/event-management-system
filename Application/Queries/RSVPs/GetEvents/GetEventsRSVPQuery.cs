using System;
using System.Collections.Generic;

using Application.Models;

namespace Application.Queries.RSVPs;

public sealed record GetEventsRSVPQuery(Guid EventId, Guid OwnerId)
    : IRequest<GetEventsRSVPQueryResult>;

public sealed record GetEventsRSVPQueryResult(IEnumerable<RSVPQueryModel> RSVPs);