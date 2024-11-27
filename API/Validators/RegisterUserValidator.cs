using Application.Commands.Auth;

using FluentValidation;

namespace API.Validators
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserValidator()
        {
            _ = RuleFor(x => x.Email).EmailAddress();
            _ = RuleFor(x => x.Password).MinimumLength(6);
        }
    }
}