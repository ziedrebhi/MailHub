using FluentValidation;
using MailHub.Application.EmailQueue.Commands;
using MailHub.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MailHub.Application.EmailQueue.Validators
{

    public class AddEmailToQueueCommandValidator : AbstractValidator<AddEmailToQueueCommand>
    {
        private readonly MailHubDbContext _dbContext;

        public AddEmailToQueueCommandValidator(MailHubDbContext dbContext)
        {
            _dbContext = dbContext;

            // Validate Recipient
            RuleFor(x => x.Recipient)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid email address is required.");

            // Ensure either TemplateId or TemplateName is provided
            RuleFor(x => x.TemplateId)
                .GreaterThan(0)
                .When(x => string.IsNullOrEmpty(x.TemplateName)) // Only check TemplateId if TemplateName is not provided
                .WithMessage("TemplateId must be greater than 0.")
                .MustAsync(TemplateExistsById)
                .WithMessage("Template does not exist with the provided TemplateId.");

            RuleFor(x => x.TemplateName)
                .NotEmpty()
                .When(x => !x.TemplateId.HasValue) // Only check TemplateName if TemplateId is not provided
                .WithMessage("TemplateName must be provided if TemplateId is not specified.")
                .MustAsync(TemplateExistsByName)
                .WithMessage("Template does not exist with the provided TemplateName.");

        }


        // Validate if template exists by Id
        private async Task<bool> TemplateExistsById(int? templateId, CancellationToken cancellationToken)
        {
            return await _dbContext.EmailTemplates.AnyAsync(t => t.Id == templateId && !t.IsDeleted, cancellationToken);
        }

        // Validate if template exists by Name
        private async Task<bool> TemplateExistsByName(string templateName, CancellationToken cancellationToken)
        {
            return await _dbContext.EmailTemplates.AnyAsync(t => t.Name == templateName && !t.IsDeleted, cancellationToken);
        }
    }


}


