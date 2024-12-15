namespace Application.Commands.Auth
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).MinimumLength(6);
        }
    }
}