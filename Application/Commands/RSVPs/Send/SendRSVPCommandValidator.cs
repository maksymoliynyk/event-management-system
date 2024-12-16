namespace Application.Commands.RSVPs;

public class SendRSVPCommandValidator : AbstractValidator<SendRSVPCommand>
{
    public SendRSVPCommandValidator()
    {
        RuleFor(x => x.UserInviteEmail).EmailAddress();
    }
}