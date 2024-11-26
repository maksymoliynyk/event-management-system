using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Infrastructure;

using MediatR;

namespace Application.Queries.RSVPQueries;

public class GetUsersRSVPQuery : IRequest<GetUsersRSVPResult>
{
    public string UserId { get; init; }
}

public class GetUsersRSVPResult
{
    public IEnumerable<RSVP> RSVPs { get; init; }
}
public class GetUsersRSVPHandler : IRequestHandler<GetUsersRSVPQuery, GetUsersRSVPResult>
{

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetUsersRSVPHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetUsersRSVPResult> Handle(GetUsersRSVPQuery request, CancellationToken cancellationToken)
    {
        return new GetUsersRSVPResult
        {
            RSVPs = _mapper.Map<IEnumerable<RSVP>>(await _unitOfWork.RSVP.GetAllRSVPsForUser(request.UserId, cancellationToken))
        };
    }
}
