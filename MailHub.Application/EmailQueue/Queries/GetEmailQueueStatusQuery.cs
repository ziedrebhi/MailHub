using MailHub.Application.EmailQueue.DTOs;
using MediatR;

namespace MailHub.Application.EmailQueue.Queries
{
    public class GetEmailQueueStatusQuery : IRequest<EmailQueueStatusDto>
    {
        public int Id { get; set; }

        public GetEmailQueueStatusQuery(int id)
        {
            Id = id;
        }
    }
}
