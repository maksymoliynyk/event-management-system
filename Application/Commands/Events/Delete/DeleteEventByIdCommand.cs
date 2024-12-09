using System;

namespace Application.Commands.Events.Delete;

public sealed record DeleteEventByIdCommand(Guid Id, Guid UserId) : IRequest<DeleteEventByIdResult>;

public sealed record DeleteEventByIdResult;