using Contracts.RequestModels;

using FluentValidation;

namespace API.Validators
{
    public class CreateEventValidator : AbstractValidator<CreateEventRequest>
    {
        public CreateEventValidator()
        {
            _ = RuleFor(x => x.Title).Length(1, 100);
            _ = RuleFor(x => x.Description).Length(1, 500);
            _ = RuleFor(x => x.Date).NotEmpty();
            _ = RuleFor(x => x.Duration).NotEmpty();
            _ = RuleFor(x => x.Location).Length(1, 100);
            _ = RuleFor(x => x.IsPublic).NotEmpty();
        }
    }
}