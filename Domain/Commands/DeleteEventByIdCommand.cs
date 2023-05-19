using System.Threading;
using System.Threading.Tasks;

using Domain.Interfaces;

using MediatR;

namespace Domain.Commands
{
    public class DeleteEventByIdCommand : IRequest<DeleteEventByIdResult>
    {
        public string Id { get; init; }
    }
    public class DeleteEventByIdResult
    {

    }
    public class DeleteEventByIdCommandHandler : IRequestHandler<DeleteEventByIdCommand, DeleteEventByIdResult>
    {

        private readonly IRepositoryManager _repositoryManager;

        public DeleteEventByIdCommandHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<DeleteEventByIdResult> Handle(DeleteEventByIdCommand request, CancellationToken cancellationToken)
        {
            _ = await _repositoryManager.Event.DeleteEventById(request.Id, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);
            return new DeleteEventByIdResult();
        }
    }
}