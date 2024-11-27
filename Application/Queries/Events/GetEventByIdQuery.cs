using System;
using System.Threading;
using System.Threading.Tasks;

using Application.Interfaces;
using Application.Queries.Models;

using MediatR;

namespace Application.Queries.Events;

public sealed record GetEventByIdQuery(Guid Id, Guid UserId) : IRequest<GetEventByIdQueryResult>;

public sealed record GetEventByIdQueryResult(EventQueryModel Event);

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, GetEventByIdQueryResult>
{
    private readonly IEventQueryRepository _repository;

    public GetEventByIdQueryHandler(IEventQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetEventByIdQueryResult> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetEventById(request.Id, request.UserId, cancellationToken);
        return new GetEventByIdQueryResult(result);
    }
}