using System.Threading;
using System.Threading.Tasks;

using Domain.Interfaces;

using MediatR;

namespace Domain.Commands
{
    public class DeleteEventByIdCommand : IRequest
    {
        public string Id { get; init; }
    }
    public class DeleteEventByIdCommandHandler : IRequestHandler<DeleteEventByIdCommand>
    {
        // private readonly IRepository _repository;

        // public DeleteEventByIdCommandHandler(IRepository repository)
        // {
        //     _repository = repository;
        // }

        private readonly IRepositoryManager _repositoryManager;

        public DeleteEventByIdCommandHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task Handle(DeleteEventByIdCommand request, CancellationToken cancellationToken)
        {
            _ = await _repositoryManager.Event.DeleteEventById(request.Id, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);
            return;
        }
    }
}