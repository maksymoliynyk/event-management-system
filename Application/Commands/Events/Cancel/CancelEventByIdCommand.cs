using System;

namespace Application.Commands.Events;

public sealed record CancelEventByIdCommand(Guid Id, Guid UserId) : IRequest<CancelEventByIdResult>;

public sealed record CancelEventByIdResult;