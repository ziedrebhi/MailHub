using MediatR;

namespace MailHub.Application.Email.Commands
{
    public class SendEmailCommand : IRequest<bool>
    {
        public int EmailQueueId { get; set; }
    }
}
