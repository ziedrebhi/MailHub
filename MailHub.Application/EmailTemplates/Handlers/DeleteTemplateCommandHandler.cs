using MailHub.Application.EmailTemplates.Commands;
using MailHub.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MailHub.Application.EmailTemplates.Handlers
{
    public class DeleteTemplateCommandHandler : IRequestHandler<DeleteTemplateCommand, bool>
    {
        private readonly MailHubDbContext _dbContext;

        public DeleteTemplateCommandHandler(MailHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _dbContext.EmailTemplates
                .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

            if (template == null)
                return false; // Return false if the template is not found or has been deleted.

            template.IsDeleted = true; // Soft delete
            _dbContext.EmailTemplates.Update(template);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true; // Return true if the soft delete was successful
        }
    }
}
