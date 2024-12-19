using MailHub.Domain.Common;
using MailHub.Domain.Enums;

namespace MailHub.Domain.Entities
{

    public class EmailLog : BaseEntity
    {
        public int Id { get; set; }
        public int EmailQueueId { get; set; }
        public EmailStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;

        public EmailQueue EmailQueue { get; set; }
    }
}
