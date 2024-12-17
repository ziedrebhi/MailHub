using MediatR;

namespace MailHub.Application.EmailTemplates.Commands
{
    public class DeleteTemplateCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
