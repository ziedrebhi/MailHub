using MailHub.Application.Interfaces;
using MailHub.Domain.Entities;
using MailHub.Domain.Enums;
using MailHub.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Text.Json;

namespace MailHub.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly MailHubDbContext _dbContext;
        private readonly IEncryptionService _encryptionService;
        public EmailSender(MailHubDbContext dbContext, IEncryptionService encryptionService)
        {
            _dbContext = dbContext;
            _encryptionService = encryptionService;
        }

        public async Task<bool> SendEmailAsync(int emailQueueId, CancellationToken cancellationToken)
        {
            var emailQueue = await _dbContext.EmailQueues
                .Include(e => e.Template)
                .FirstOrDefaultAsync(e => e.Id == emailQueueId && e.Status == EmailStatus.Pending, cancellationToken);

            if (emailQueue == null)
                return false;

            var emailConfig = await _dbContext.EmailConfigurations
                .FirstOrDefaultAsync(c => c.IsDefault, cancellationToken);

            if (emailConfig == null)
                throw new Exception("No default email configuration found.");

            try
            {
                var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>(emailQueue.Parameters);
                var subject = emailQueue.Template.Subject;
                var body = emailQueue.Template.Body;

                foreach (var param in parameters)
                {
                    body = body.Replace($"{{{{{param.Key}}}}}", param.Value);
                }

                using var smtpClient = new SmtpClient(emailConfig.SmtpHost, emailConfig.SmtpPort)
                {
                    EnableSsl = emailConfig.EnableSsl,
                    Credentials = new NetworkCredential(emailConfig.SenderEmail, _encryptionService.Decrypt(emailConfig.SenderPassword)),

                };

                var mailMessage = new MailMessage(emailConfig.SenderEmail, emailQueue.Recipient, subject, body)
                {
                    IsBodyHtml = true
                };

                await smtpClient.SendMailAsync(mailMessage);

                emailQueue.Status = EmailStatus.Sent;
                emailQueue.SentAt = DateTime.UtcNow;
                _dbContext.EmailLogs.Add(new EmailLog
                {
                    EmailQueueId = emailQueue.Id,
                    Status = EmailStatus.Sent,
                    AttemptedAt = DateTime.UtcNow
                });

                await _dbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception ex)
            {
                emailQueue.Status = EmailStatus.Failed;
                emailQueue.ErrorMessage = ex.Message;

                _dbContext.EmailLogs.Add(new EmailLog
                {
                    EmailQueueId = emailQueue.Id,
                    Status = EmailStatus.Failed,
                    ErrorMessage = ex.Message,
                    AttemptedAt = DateTime.UtcNow
                });

                await _dbContext.SaveChangesAsync(cancellationToken);
                return false;
            }
        }
    }
}
