using MailHub.Application.EmailQueue.DTOs;
using MailHub.Domain.Enums;
using MediatR;

namespace MailHub.Application.EmailQueue.Queries
{
    public class GetAllEmailQueueStatusQuery : IRequest<List<EmailQueueStatusDto>>
    {
        public EmailStatus? Status { get; set; }

        public GetAllEmailQueueStatusQuery(EmailStatus? status = null)
        {
            Status = status;
        }
    }
}
