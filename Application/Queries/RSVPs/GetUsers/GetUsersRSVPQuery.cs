using System;
using System.Collections.Generic;

using Application.Models;

namespace Application.Queries.RSVPs;

public sealed record GetUsersRSVPQuery(Guid UserId)
    : IRequest<GetUsersRSVPQueryResult>;

public sealed record GetUsersRSVPQueryResult(IEnumerable<RSVPQueryModel> RSVPs);