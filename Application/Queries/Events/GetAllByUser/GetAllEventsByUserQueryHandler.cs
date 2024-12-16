using Application.Interfaces.Repositories;

namespace Application.Queries.Events;

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