using System;

namespace Application.Commands.RSVPs.Send;

public sealed record SendRSVPCommand(Guid UserOwnerId, string UserInviteEmail, Guid EventId) : IRequest<SendRSVPResult>;

public sealed record SendRSVPResult(Guid RSVPId);