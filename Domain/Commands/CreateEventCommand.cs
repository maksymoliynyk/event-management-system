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
        public DateTime Date { get; init; }
        public long Duration { get; init; }
        public string Location { get; init; }
        public bool IsPublic { get; init; }
        public string UserName { get; init; }
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
            string userId = _repositoryManager.User.GetUserByUsername(request.UserName, cancellationToken).Result.Id;
            EventDTO newEvent = new()
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Date = request.Date,
                Duration = TimeSpan.FromSeconds(request.Duration),
                Location = request.Location,
                OwnerId = userId,
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