using MailHub.Application.EmailTemplates.DTOs;
using MailHub.Application.EmailTemplates.Queries;
using MailHub.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MailHub.Application.EmailTemplates.Handlers
{
    public class GetTemplateByIdQueryHandler : IRequestHandler<GetTemplateByIdQuery, EmailTemplateDto>
    {
        private readonly MailHubDbContext _dbContext;

        public GetTemplateByIdQueryHandler(MailHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EmailTemplateDto> Handle(GetTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            // Retrieve the email template by ID
            var template = await _dbContext.EmailTemplates
                .Where(t => t.Id == request.Id && !t.IsDeleted) // Soft delete check
                .FirstOrDefaultAsync(cancellationToken);

            if (template == null)
                return null; // Return null if template not found

            // Map the entity to a DTO
            var templateDto = new EmailTemplateDto
            {
                Id = template.Id,
                Name = template.Name,
                Subject = template.Subject,
                Body = template.Body,
                Parameters = template.Parameters
            };

            return templateDto; // Return the mapped DTO
        }
    }
}