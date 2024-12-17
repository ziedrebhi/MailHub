using MailHub.Application.EmailTemplates.Commands;
using MailHub.Domain.Entities;
using MailHub.Persistence.Context;
using MediatR;

namespace MailHub.Application.EmailTemplates.Handlers
{
    public class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, int>
    {
        private readonly MailHubDbContext _dbContext;

        public CreateTemplateCommandHandler(MailHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = new EmailTemplate
            {
                Name = request.Name,
                Subject = request.Subject,
                Body = request.Body,
                Parameters = request.Parameters
            };

            await _dbContext.EmailTemplates.AddAsync(template, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return template.Id;  // Return template ID after creation
        }
    }
}
