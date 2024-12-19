namespace MailHub.Application.Interfaces
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(int emailQueueId, CancellationToken cancellationToken);
    }
}
