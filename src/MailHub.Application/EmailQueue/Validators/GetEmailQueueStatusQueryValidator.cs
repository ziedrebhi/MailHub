using FluentValidation;
using MailHub.Application.EmailQueue.Queries;

namespace MailHub.Application.EmailQueue.Validators
{
    public class GetEmailQueueStatusQueryValidator : AbstractValidator<GetEmailQueueStatusQuery>
    {
        public GetEmailQueueStatusQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("EmailQueue ID must be a positive integer.");
        }
    }
}
