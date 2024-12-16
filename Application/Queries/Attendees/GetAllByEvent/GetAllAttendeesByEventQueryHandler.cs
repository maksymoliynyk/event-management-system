using Application.Interfaces.Repositories;

namespace Application.Queries.Attendees;

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
