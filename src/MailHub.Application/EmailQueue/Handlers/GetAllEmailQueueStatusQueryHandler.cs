using MailHub.Application.EmailQueue.DTOs;
using MailHub.Application.EmailQueue.Queries;
using MailHub.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MailHub.Application.EmailQueue.Handlers
{
    public class GetAllEmailQueueStatusQueryHandler : IRequestHandler<GetAllEmailQueueStatusQuery, List<EmailQueueStatusDto>>
    {
        private readonly MailHubDbContext _dbContext;

        public GetAllEmailQueueStatusQueryHandler(MailHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<EmailQueueStatusDto>> Handle(GetAllEmailQueueStatusQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.EmailQueues.AsQueryable();

            if (request.Status.HasValue)
            {
                query = query.Where(e => e.Status == request.Status.Value);
            }

            var emailQueues = await query
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync(cancellationToken);

            return emailQueues.Select(e => new EmailQueueStatusDto
            {
                Id = e.Id,
                Status = e.Status,
                Recipient = e.Recipient,
                ErrorMessage = e.ErrorMessage,
                SentAt = e.SentAt,
                Parameters = e.Parameters,
                TemplateId = e.TemplateId
            }).ToList();
        }
    }
}
