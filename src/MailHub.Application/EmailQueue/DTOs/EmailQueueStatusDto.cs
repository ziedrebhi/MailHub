using MailHub.Domain.Enums;

namespace MailHub.Application.EmailQueue.DTOs
{
    public class EmailQueueStatusDto
    {
        public int Id { get; set; }
        public EmailStatus Status { get; set; }
        public string Recipient { get; set; }
        public string Parameters { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? SentAt { get; set; }
        public int TemplateId { get; set; }
    }
}
