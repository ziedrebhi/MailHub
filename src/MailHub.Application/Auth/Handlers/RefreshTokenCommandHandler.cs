using MailHub.Application.Auth.Commands;
using MailHub.Application.Auth.DTOs;
using MailHub.Application.Interfaces;
using MailHub.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MailHub.Application.Auth.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
    {
        private readonly MailHubDbContext _dbContext;
        private readonly IJWTService _jwtService;

        public RefreshTokenCommandHandler(MailHubDbContext dbContext, IJWTService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var userIdFromToken = _jwtService.GetUserIdFromToken(request.Token);

            var userToken = await _dbContext.UserTokens
                .Include(ut => ut.User)
                .FirstOrDefaultAsync(ut => ut.UserId == userIdFromToken && ut.RefreshToken == request.RefreshToken, cancellationToken);

            if (userToken == null || userToken.ExpiryDate <= DateTime.UtcNow)
            {
                throw new Exception("Invalid or expired refresh token");
            }

            // Generate new JWT and refresh token
            var newToken = _jwtService.GenerateJwtToken(userToken.UserId, userToken.User.Role);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            userToken.RefreshToken = newRefreshToken;
            userToken.ExpiryDate = DateTime.UtcNow.AddMonths(1);
            _dbContext.UserTokens.Update(userToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new LoginResponse
            {
                Token = newToken,
                RefreshToken = newRefreshToken
            };
        }
    }

}
