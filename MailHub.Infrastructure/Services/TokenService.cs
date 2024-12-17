using MailHub.Application.Interfaces;

namespace MailHub.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        public string GenerateRefreshToken()
        {
            // Generate a secure random string for the refresh token
            return Guid.NewGuid().ToString();
        }
    }
}
