using Domain.Commands.AuthCommands;

using FluentValidation;

namespace API.Validators
{
    public class LoginUserValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserValidator()
        {
            _ = RuleFor(x => x.Email).EmailAddress();
            _ = RuleFor(x => x.Password).Length(1, 50);
        }
    }
}