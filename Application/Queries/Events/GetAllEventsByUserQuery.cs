using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Application.Enums;
using Application.Interfaces;
using Application.Queries.Models;

using MediatR;

namespace Application.Queries.Events;

public sealed record GetAllEventsByUserQuery(Guid UserId, EventQueryingMode Mode)
    : IRequest<GetAllEventsByUserQueryResult>;

public sealed record GetAllEventsByUserQueryResult(IEnumerable<EventQueryModel> Events);

public class GetAllEventsByUserQueryHandler : IRequestHandler<GetAllEventsByUserQuery, GetAllEventsByUserQueryResult>
{
    private readonly IEventQueryRepository _repository;

    public GetAllEventsByUserQueryHandler(IEventQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAllEventsByUserQueryResult> Handle(GetAllEventsByUserQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllEventsForUser(request.UserId, request.Mode, cancellationToken);
        return new GetAllEventsByUserQueryResult(result);
    }
}