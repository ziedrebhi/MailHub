using MailHub.Application.EmailTemplates.Commands;
using MailHub.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MailHub.Application.EmailTemplates.Handlers
{
    public class UpdateTemplateCommandHandler : IRequestHandler<UpdateTemplateCommand, bool>
    {
        private readonly MailHubDbContext _dbContext;

        public UpdateTemplateCommandHandler(MailHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _dbContext.EmailTemplates
                .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

            if (template == null)
                return false; // Return false if the template is not found or has been deleted.

            template.Name = request.Name;
            template.Subject = request.Subject;
            template.Body = request.Body;
            template.Parameters = request.Parameters;

            _dbContext.EmailTemplates.Update(template);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true; // Return true if the update was successful
        }
    }
}
