using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Infrastructure;

using MediatR;

namespace Application.Queries.UserQueries;

public class GetAllEventsWhereUserAreParticipateQuery : IRequest<GetAllEventsWhereUserAreParticipateResult>
{
    public string UserId { get; init; }
}

public class GetAllEventsWhereUserAreParticipateResult
{
    public IEnumerable<Event> Events { get; init; }
}

public class GetAllEventsWhereUserAreParticipateHandler : IRequestHandler<GetAllEventsWhereUserAreParticipateQuery, GetAllEventsWhereUserAreParticipateResult>
{
    private readonly IMapper _mapper;

    private readonly IUnitOfWork _unitOfWork;
    public GetAllEventsWhereUserAreParticipateHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }
    public async Task<GetAllEventsWhereUserAreParticipateResult> Handle(GetAllEventsWhereUserAreParticipateQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Event> events = _mapper.Map<IEnumerable<Event>>(await _unitOfWork.Event.GetAllEventsWhereUserAreParticipate(request.UserId, cancellationToken));
        return new GetAllEventsWhereUserAreParticipateResult
        {
            Events = events
        };
    }
}
