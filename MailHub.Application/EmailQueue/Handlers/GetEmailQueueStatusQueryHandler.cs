using MailHub.Application.EmailQueue.DTOs;
using MailHub.Application.EmailQueue.Queries;
using MailHub.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MailHub.Application.EmailQueue.Handlers
{
    public class GetEmailQueueStatusQueryHandler : IRequestHandler<GetEmailQueueStatusQuery, EmailQueueStatusDto>
    {
        private readonly MailHubDbContext _dbContext;

        public GetEmailQueueStatusQueryHandler(MailHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EmailQueueStatusDto> Handle(GetEmailQueueStatusQuery request, CancellationToken cancellationToken)
        {
            var emailQueue = await _dbContext.EmailQueues
                .Where(e => e.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (emailQueue == null)
            {
                throw new Exception("Email not found in the queue.");
            }

            return new EmailQueueStatusDto
            {
                Id = emailQueue.Id,
                Status = emailQueue.Status,
                Recipient = emailQueue.Recipient,
                ErrorMessage = emailQueue.ErrorMessage,
                SentAt = emailQueue.SentAt,
                Parameters = emailQueue.Parameters,
                TemplateId = emailQueue.TemplateId
            };
        }
    }
}
