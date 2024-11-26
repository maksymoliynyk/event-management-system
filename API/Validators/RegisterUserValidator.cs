using Application.Commands.AuthCommands;

using FluentValidation;

namespace API.Validators
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            _ = RuleFor(x => x.Email).EmailAddress();
            _ = RuleFor(x => x.Username).Length(1, 50);
            _ = RuleFor(x => x.Password).MinimumLength(6);
        }
    }
}