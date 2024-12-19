using MailHub.Application.Auth.DTOs;
using MediatR;

namespace MailHub.Application.Auth.Commands
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
