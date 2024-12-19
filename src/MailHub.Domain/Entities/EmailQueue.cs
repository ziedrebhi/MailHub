using MailHub.Domain.Common;
using MailHub.Domain.Enums;

namespace MailHub.Domain.Entities
{
    public class EmailQueue : BaseEntity
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public string Recipient { get; set; }
        public string Parameters { get; set; }  // Serialized JSON data for dynamic values
        public EmailStatus Status { get; set; } = EmailStatus.Pending;
        public string? ErrorMessage { get; set; }
        public DateTime? SentAt { get; set; }

        public EmailTemplate Template { get; set; }
    }
}
