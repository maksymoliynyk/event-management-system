using System;
using System.Threading;
using System.Threading.Tasks;

using Domain.Interfaces;
using Domain.Models.Database;

using MediatR;

namespace Domain.Commands
{
    public class CreateEventCommand : IRequest<CreateEventResult>
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public DateTimeOffset Date { get; init; }
        public long Duration { get; init; }
        public string Location { get; init; }
        public string OwnerEmail { get; init; }
    }

    public class CreateEventResult
    {
        public string Id { get; init; }
    }

    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, CreateEventResult>
    {
        private readonly IRepositoryManager _repositoryManager;

        public CreateEventCommandHandler(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<CreateEventResult> Handle(CreateEventCommand request, CancellationToken cancellationToken = default)
        {
            UserDTO user = await _repositoryManager.User.GetUserByEmailOrCreateUser(request.OwnerEmail, cancellationToken);
            EventDTO newEvent = new()
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Date = request.Date,
                Duration = TimeSpan.FromSeconds(request.Duration),
                Location = request.Location,
                OwnerId = user.Id,
                Status = 0
            };
            string id = await _repositoryManager.Event.CreateEvent(newEvent, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);
            return new CreateEventResult
            {
                Id = id
            };
        }
    }
}