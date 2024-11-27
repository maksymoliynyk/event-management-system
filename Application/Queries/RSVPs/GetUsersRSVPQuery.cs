using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Application.Interfaces;
using Application.Queries.Models;

using MediatR;

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