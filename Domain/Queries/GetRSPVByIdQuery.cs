using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Interfaces;

using MediatR;

namespace Domain.Queries
{
    public class GetRSPVByIdQuery : IRequest<GetRSPVByIdResult>
    {
        public string RSPVId { get; init; }
    }

    public class GetRSPVByIdResult
    {
        public RSPV RSPV { get; init; }
    }
    public class GetRSPVByIdQueryHandler : IRequestHandler<GetRSPVByIdQuery, GetRSPVByIdResult>
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;

        public GetRSPVByIdQueryHandler(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
        }

        public async Task<GetRSPVByIdResult> Handle(GetRSPVByIdQuery request, CancellationToken cancellationToken)
        {
            RSPV rspv = _mapper.Map<RSPV>(await _repositoryManager.RSPV.GetRSPVById(request.RSPVId, cancellationToken));
            return new GetRSPVByIdResult
            {
                RSPV = rspv
            };
        }
    }
}