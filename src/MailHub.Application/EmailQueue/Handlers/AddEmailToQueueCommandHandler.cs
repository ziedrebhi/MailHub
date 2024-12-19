using MailHub.Application.EmailQueue.Commands;
using MailHub.Domain.Entities;
using MailHub.Domain.Enums;
using MailHub.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MailHub.Application.EmailQueue.Handlers
{
    public class AddEmailToQueueCommandHandler : IRequestHandler<AddEmailToQueueCommand, bool>
    {
        private readonly MailHubDbContext _dbContext;

        public AddEmailToQueueCommandHandler(MailHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(AddEmailToQueueCommand request, CancellationToken cancellationToken)
        {
            // Fetch the template by Id or Name (if Id is not provided)
            EmailTemplate template = await GetTemplateAsync(request, cancellationToken);

            // Deserialize the template's parameters for validation (if any)
            List<string> templateParameters = template.Parameters;

            // Validate template parameters
            await ValidateTemplateParametersAsync(templateParameters, request.Parameters);

            // Create a new EmailQueue entry
            var emailQueue = new Domain.Entities.EmailQueue
            {
                TemplateId = template.Id,
                Recipient = request.Recipient,
                Parameters = request.Parameters != null ? JsonSerializer.Serialize(request.Parameters) : null,
                Status = EmailStatus.Pending
            };

            await _dbContext.EmailQueues.AddAsync(emailQueue, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        private async Task<EmailTemplate> GetTemplateAsync(AddEmailToQueueCommand request, CancellationToken cancellationToken)
        {
            EmailTemplate template;

            if (request.TemplateId.HasValue)
            {
                // Fetch template by Id
                template = await _dbContext.EmailTemplates
                    .FirstOrDefaultAsync(t => t.Id == request.TemplateId && !t.IsDeleted, cancellationToken);
            }
            else if (!string.IsNullOrEmpty(request.TemplateName))
            {
                // Fetch template by Name
                template = await _dbContext.EmailTemplates
                    .FirstOrDefaultAsync(t => t.Name == request.TemplateName && !t.IsDeleted, cancellationToken);
            }
            else
            {
                throw new Exception("Template Id or Template Name must be provided.");
            }

            if (template == null)
            {
                throw new Exception("Template not found.");
            }

            return template;
        }

        private async Task ValidateTemplateParametersAsync(List<string> templateParameters, Dictionary<string, string> requestParameters)
        {
            if (templateParameters != null && templateParameters.Any())
            {
                // Validate that parameters are provided
                if (requestParameters == null || !requestParameters.Any())
                    throw new Exception($"Parameters are required for this template: {string.Join(", ", templateParameters)}");

                // Check if any required parameters are missing
                var missingParameters = templateParameters.Except(requestParameters.Keys).ToList();
                if (missingParameters.Any())
                    throw new Exception($"Missing required parameters: {string.Join(", ", missingParameters)}");

                // Check for any unexpected parameters provided by the user
                var extraParameters = requestParameters.Keys.Except(templateParameters).ToList();
                if (extraParameters.Any())
                    throw new Exception($"Unexpected parameters provided: {string.Join(", ", extraParameters)}");
            }
            else
            {
                // If the template has no parameters, ensure no parameters are provided in the request
                if (requestParameters != null && requestParameters.Any())
                    throw new Exception("No parameters should be provided for this template.");
            }
        }
    }

}
