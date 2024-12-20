using MailHub.Application.Auth.DTOs;
using MediatR;

namespace MailHub.Application.Auth.Commands
{
    public class RefreshTokenCommand : IRequest<LoginResponse>
    {
        public string Token { get; set; } // The current (expired) access token
        public string RefreshToken { get; set; } // The refresh token
    }
}
