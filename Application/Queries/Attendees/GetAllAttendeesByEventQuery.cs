using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Application.Interfaces;
using Application.Queries.Models;

using MediatR;

namespace Application.Queries.Attendees;

public sealed record GetAllAttendeesByEventQuery(Guid EventId, Guid OwnerId)
    : IRequest<GetAllAttendeesByEventQueryResult>;

public sealed record GetAllAttendeesByEventQueryResult(IEnumerable<AttendeeQueryModel> Attendees);

public class GetAllAttendeesByEventQueryHandler : IRequestHandler<GetAllAttendeesByEventQuery, GetAllAttendeesByEventQueryResult>
{
    private readonly IEventQueryRepository _repository;

    public GetAllAttendeesByEventQueryHandler(IEventQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAllAttendeesByEventQueryResult> Handle(GetAllAttendeesByEventQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllAttendeesByEvent(request.EventId, request.OwnerId, cancellationToken);
        return new GetAllAttendeesByEventQueryResult(result);
    }
}
