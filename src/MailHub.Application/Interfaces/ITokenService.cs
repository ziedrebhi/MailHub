namespace MailHub.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateRefreshToken();
    }
}