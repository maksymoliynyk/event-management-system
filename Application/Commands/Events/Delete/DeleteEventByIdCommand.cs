using System;

namespace Application.Commands.Events;

public sealed record DeleteEventByIdCommand(Guid Id, Guid UserId) : IRequest<DeleteEventByIdResult>;

public sealed record DeleteEventByIdResult;