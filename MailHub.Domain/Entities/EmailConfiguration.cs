using MailHub.Domain.Common;

namespace MailHub.Domain.Entities
{
    public class EmailConfiguration : BaseEntity
    {
        public int Id { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }  // Store encrypted password
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
        public bool IsDefault { get; set; } = false;  // To mark the default configuration
    }
}
