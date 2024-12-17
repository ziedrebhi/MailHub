using MailHub.Application.Email.Commands;
using MailHub.Application.Interfaces;
using MediatR;

namespace MailHub.Application.Email.Handlers
{

    public class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, bool>
    {
        private readonly IEmailSender _emailSender;

        public SendEmailCommandHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task<bool> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            return await _emailSender.SendEmailAsync(request.EmailQueueId, cancellationToken);
        }
    }
}