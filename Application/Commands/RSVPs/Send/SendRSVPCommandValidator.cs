namespace Application.Commands.RSVPs.Send;

public class SendRSVPCommandValidator : AbstractValidator<SendRSVPCommand>
{
    public SendRSVPCommandValidator()
    {
        RuleFor(x => x.UserInviteEmail).EmailAddress();
    }
}