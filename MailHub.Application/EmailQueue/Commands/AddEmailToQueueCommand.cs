using MediatR;

namespace MailHub.Application.EmailQueue.Commands
{
    public class AddEmailToQueueCommand : IRequest<bool>
    {
        public int? TemplateId { get; set; } // Optional TemplateId
        public string TemplateName { get; set; } // Optional TemplateName
        public string Recipient { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
}
