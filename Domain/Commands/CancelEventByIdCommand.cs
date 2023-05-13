using System.Threading;
using System.Threading.Tasks;

using Contracts.Models.Statuses;

using Domain.Repositories;

using MediatR;

namespace Domain.Commands
{
    public class CancelEventByIdCommand : IRequest
    {
        public string Id { get; init; }
    }
    public class CancelEventByIdCommandHandler : IRequestHandler<CancelEventByIdCommand>
    {
        private readonly IRepository _repository;

        public CancelEventByIdCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(CancelEventByIdCommand request, CancellationToken cancellationToken)
        {
            await _repository.ChangeEventStatus(request.Id, EventStatus.Cancelled, cancellationToken);
            return;
        }
    }
}