using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Contracts.Models;

using Domain.Repositories;

using MediatR;

namespace Domain.Queries
{
    public class GetEventByIdQuery : IRequest<GetEventByIdResult>
    {
        public string Id { get; init; }
    }

    public class GetEventByIdResult
    {
        public Event SearchedEvent { get; init; }

    }
    public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, GetEventByIdResult>
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public GetEventByIdQueryHandler(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GetEventByIdResult> Handle(GetEventByIdQuery request, CancellationToken cancellationToken = default)
        {
            Event searchedEvent = _mapper.Map<Event>(await _repository.GetEventById(request.Id, cancellationToken));
            return new GetEventByIdResult
            {
                SearchedEvent = searchedEvent
            };
        }
    }
}
