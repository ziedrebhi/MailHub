using FluentValidation;
using MailHub.Application.EmailTemplates.Commands;

namespace MailHub.Application.EmailTemplates.Validators
{
    public class DeleteTemplateCommandValidator : AbstractValidator<DeleteTemplateCommand>
    {
        public DeleteTemplateCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Invalid template ID.");
        }
    }
}
