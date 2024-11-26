using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Aggregates.Events;
using Domain.Exceptions;

using Infrastructure;

using MediatR;

namespace Application.Queries.RSVPQueries;

public class GetRSVPByIdQuery : IRequest<GetRSVPByIdResult>
{
    public string RSVPId { get; init; }
    public string UserId { get; init; }
}

public class GetRSVPByIdResult
{
    public RSVP RSVP { get; init; }
}
public class GetRSVPByIdHandler : IRequestHandler<GetRSVPByIdQuery, GetRSVPByIdResult>
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetRSVPByIdHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetRSVPByIdResult> Handle(GetRSVPByIdQuery request, CancellationToken cancellationToken)
    {
        RSVPDTO rspvDTO = await _unitOfWork.RSVP.GetRSVPById(request.RSVPId, cancellationToken);
        bool hasNoAccess = !(await _unitOfWork.Event.IsUserOwner(request.UserId, rspvDTO.EventId, cancellationToken)
                || await _unitOfWork.RSVP.IsUserInvited(request.UserId, rspvDTO.EventId, cancellationToken));
        return hasNoAccess
            ? throw new NoAccessException("You have no access to this RSVP")
            : new GetRSVPByIdResult
            {
                RSVP = _mapper.Map<RSVP>(rspvDTO)
            };
    }
}