using System.Threading;
using System.Threading.Tasks;

using Domain.Repositories;

using MediatR;

namespace Domain.Commands
{
    public class DeleteEventByIdCommand : IRequest
    {
        public string Id { get; init; }
    }
    public class DeleteEventByIdCommandHandler : IRequestHandler<DeleteEventByIdCommand>
    {
        private readonly IRepository _repository;

        public DeleteEventByIdCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteEventByIdCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteEventById(request.Id, cancellationToken);
            return;
        }
    }
}