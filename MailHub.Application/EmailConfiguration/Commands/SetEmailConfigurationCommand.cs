using MediatR;

namespace MailHub.Application.EmailConfiguration.Commands
{
    public class SetEmailConfigurationCommand : IRequest<bool>
    {
        public int? Id { get; set; } // Nullable for creating new or updating existing configuration
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl { get; set; }
        public bool IsDefault { get; set; }
    }
}
