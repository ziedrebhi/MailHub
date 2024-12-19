using MediatR;

namespace MailHub.Application.Auth.Commands
{
    public class SignUpCommand : IRequest<int>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
