using MailHub.Application.Auth.Commands;
using MailHub.Domain.Entities;
using MailHub.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MailHub.Application.Auth.Handlers
{
    public class SignUpCommandHandler : IRequestHandler<SignUpCommand, int>
    {
        private readonly MailHubDbContext _dbContext;

        public SignUpCommandHandler(MailHubDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> Handle(SignUpCommand request, CancellationToken cancellationToken)
        {
            // Check if user exists
            var existingUser = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
            if (existingUser != null) throw new Exception("User already exists");

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create new user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Save to DB
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}