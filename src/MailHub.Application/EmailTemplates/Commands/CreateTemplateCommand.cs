using MediatR;

namespace MailHub.Application.EmailTemplates.Commands
{
    public class CreateTemplateCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Parameters { get; set; }
    }
}
