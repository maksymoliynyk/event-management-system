using System;

namespace Application.Commands.Events;

public sealed record CreateEventCommand(
    string Title,
    string Description,
    DateTime StartDate,
    long Duration,
    string Location,
    Guid UserId)
    : IRequest<CreateEventResult>;

public sealed record CreateEventResult(Guid Id);