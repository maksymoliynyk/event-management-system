using Contracts.RequestModels;

using FluentValidation;

namespace API.Validators
{
    public class EmailRequestValidator : AbstractValidator<EmailRequest>
    {
        public EmailRequestValidator()
        {
            _ = RuleFor(x => x.Email).EmailAddress();
        }
    }
}