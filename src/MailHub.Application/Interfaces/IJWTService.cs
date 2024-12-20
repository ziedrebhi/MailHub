using System.Security.Claims;

namespace MailHub.Application.Interfaces
{
    public interface IJWTService
    {

        string GenerateJwtToken(int userId, string role);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        int GetUserIdFromToken(string token);
    }
}