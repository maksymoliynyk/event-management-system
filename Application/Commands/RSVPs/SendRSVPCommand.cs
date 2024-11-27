using System;
using System.Threading;
using System.Threading.Tasks;

using Domain.Entities.Users;
using Domain.Interfaces;

using MediatR;

namespace Application.Commands.RSVPs;

public sealed record SendRSVPCommand(Guid UserOwnerId, string UserInviteEmail, Guid EventId) : IRequest<SendRSVPResult>;

public sealed record SendRSVPResult(Guid RSVPId);

public class SendRSVPCommandHandler : IRequestHandler<SendRSVPCommand, SendRSVPResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public SendRSVPCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<SendRSVPResult> Handle(SendRSVPCommand request, CancellationToken cancellationToken)
    {
        var @event = _unitOfWork.Event.GetById(request.EventId, request.UserOwnerId);

        if (@event == null)
        {
            throw new NullReferenceException($"Event with id {request.EventId} was not found");
        }

        var invitee = await _identityService.GetUserByEmail(request.UserInviteEmail);
        var rsvpId = @event.CreateRSVP(invitee.Id);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new SendRSVPResult(rsvpId);
    }
}