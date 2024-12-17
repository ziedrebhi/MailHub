using FluentValidation;
using MailHub.Application.EmailTemplates.Commands;

namespace MailHub.Application.EmailTemplates.Validators
{
    public class CreateTemplateCommandValidator : AbstractValidator<CreateTemplateCommand>
    {
        public CreateTemplateCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Template name is required.");
            RuleFor(x => x.Subject).NotEmpty().WithMessage("Subject is required.");
            RuleFor(x => x.Body).NotEmpty().WithMessage("Body is required.");
            //RuleFor(x => x.Parameters)
            //    .Must(p => p.Count > 0).WithMessage("At least one parameter is required.");
        }
    }
}
