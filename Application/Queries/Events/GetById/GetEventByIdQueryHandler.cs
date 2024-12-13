using Application.Interfaces.Repositories;

namespace Application.Queries.Events;

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