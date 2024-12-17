using MailHub.Application.Auth.Commands;
using MailHub.Application.Auth.DTOs;
using MailHub.Application.Interfaces;
using MailHub.Domain.Entities;
using MailHub.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MailHub.Application.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly MailHubDbContext _dbContext;
        private readonly IJWTService _jwtService;
        private readonly ITokenService _tokenService;  // For managing refresh tokens

        public LoginCommandHandler(MailHubDbContext dbContext, IJWTService jwtService, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Get user from DB
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            // Generate JWT token
            var token = _jwtService.GenerateJwtToken(user.Id, user.Role);

            // Generate refresh token
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Store refresh token in the database
            var userToken = await _dbContext.UserTokens
                .FirstOrDefaultAsync(ut => ut.UserId == user.Id, cancellationToken);

            if (userToken != null)
            {
                userToken.RefreshToken = refreshToken;
                userToken.ExpiryDate = DateTime.UtcNow.AddMonths(1); // Set refresh token expiry to 1 month
                _dbContext.UserTokens.Update(userToken);
            }
            else
            {
                _dbContext.UserTokens.Add(new UserToken
                {
                    UserId = user.Id,
                    RefreshToken = refreshToken,
                    ExpiryDate = DateTime.UtcNow.AddMonths(1)
                });
            }

            await _dbContext.SaveChangesAsync(cancellationToken);

            // Return both the access token (JWT) and refresh token
            return new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken
            };
        }
    }
}