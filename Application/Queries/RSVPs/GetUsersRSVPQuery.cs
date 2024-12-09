using System;
using System.Collections.Generic;

using Application.Interfaces.Repositories;
using Application.Models;

namespace Application.Queries.RSVPs;

public sealed record GetUsersRSVPQuery(Guid UserId)
    : IRequest<GetUsersRSVPQueryResult>;

public sealed record GetUsersRSVPQueryResult(IEnumerable<RSVPQueryModel> RSVPs);

public class GetUsersRSVPQueryHandler : IRequestHandler<GetUsersRSVPQuery, GetUsersRSVPQueryResult>
{
    private readonly IEventQueryRepository _repository;

    public GetUsersRSVPQueryHandler(IEventQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetUsersRSVPQueryResult> Handle(GetUsersRSVPQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllInvitesByUser(request.UserId, cancellationToken);
        return new GetUsersRSVPQueryResult(result);
    }
}