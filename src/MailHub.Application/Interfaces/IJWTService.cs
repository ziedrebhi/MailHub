using System.Security.Claims;

namespace MailHub.Application.Interfaces
{
    public interface IJWTService
    {
        string GenerateJwtToken(int userId, string role);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}