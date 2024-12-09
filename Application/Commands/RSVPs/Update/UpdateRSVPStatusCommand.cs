using System;

namespace Application.Commands.RSVPs.Update;

public sealed record UpdateRSVPStatusCommand(bool RSVPAccepted, Guid EventId, Guid UserId)
    : IRequest<UpdateRSVPStatusResult>;

public sealed record UpdateRSVPStatusResult;