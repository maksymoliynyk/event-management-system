using System;
using System.Threading;
using System.Threading.Tasks;

using Domain.Interfaces;
using Domain.Models.Database;

using MediatR;

namespace Domain.Commands.EventCommands
{
    public class CreateEventCommand : IRequest<CreateEventResult>
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public DateTime Date { get; init; }
        public long Duration { get; init; }
        public string Location { get; init; }
        public bool IsPublic { get; init; }
        public string UserId { get; init; }
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
            EventDTO newEvent = new()
            {
                Id = Guid.NewGuid().ToString(),
                Title = request.Title,
                Description = request.Description,
                Date = request.Date,
                Duration = TimeSpan.FromSeconds(request.Duration),
                Location = request.Location,
                OwnerId = request.UserId,
                Status = 0,
                IsPublic = request.IsPublic
            };
            string eventId = await _repositoryManager.Event.CreateEvent(newEvent, cancellationToken);
            await _repositoryManager.SaveAsync(cancellationToken);
            return new CreateEventResult
            {
                Id = eventId
            };
        }
    }
}