using Application.Interfaces.Repositories;

namespace Application.Queries.RSVPs;

public class GetEventsRSVPQueryHandler : IRequestHandler<GetEventsRSVPQuery, GetEventsRSVPQueryResult>
{
    private readonly IEventQueryRepository _repository;

    public GetEventsRSVPQueryHandler(IEventQueryRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetEventsRSVPQueryResult> Handle(GetEventsRSVPQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllInvitesByEvent(request.EventId, request.OwnerId, cancellationToken);
        return new GetEventsRSVPQueryResult(result);
    }
}