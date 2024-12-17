using MailHub.Application.EmailTemplates.DTOs;
using MediatR;

namespace MailHub.Application.EmailTemplates.Queries
{
    public class GetTemplateByIdQuery : IRequest<EmailTemplateDto>
    {
        public int Id { get; set; }
    }
}
