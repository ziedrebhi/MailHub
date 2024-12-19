using MailHub.Application.Auth.Commands;
using MailHub.Application.Auth.Handlers;
using MailHub.Application.Interfaces;
using MailHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Application.Auth
{
    public class LoginCommandHandlerTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _fixture;
        private readonly Mock<IJWTService> _jwtServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
            _jwtServiceMock = new Mock<IJWTService>();
            _tokenServiceMock = new Mock<ITokenService>();
            _handler = new LoginCommandHandler(_fixture.DbContext, _jwtServiceMock.Object, _tokenServiceMock.Object);
        }

        private void ClearContext()
        {
            // Clear the change tracker to avoid tracking duplicate entities.
            _fixture.DbContext.ChangeTracker.Clear();
        }

        [Fact]
        public async Task Handle_ValidLogin_ShouldReturnTokenAndRefreshToken()
        {
            ClearContext(); // Ensure the context is cleared before the test

            // Arrange
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = "password123"
            };

            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser", // Ensure you set the 'Username'
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "Admin"
            };

            // Add user to in-memory DB
            if (!_fixture.DbContext.Users.Any(u => u.Id == user.Id))
            {
                _fixture.DbContext.Users.Add(user);
                await _fixture.DbContext.SaveChangesAsync();
            }

            // Mock JWT and Refresh Token generation
            _jwtServiceMock.Setup(x => x.GenerateJwtToken(It.IsAny<int>(), It.IsAny<string>())).Returns("jwt-token");
            _tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("new-refresh-token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("jwt-token", result.Token);
            Assert.Equal("new-refresh-token", result.RefreshToken);
        }

        [Fact]
        public async Task Handle_InvalidEmail_ShouldThrowException()
        {
            ClearContext(); // Ensure the context is cleared before the test

            // Arrange
            var command = new LoginCommand
            {
                Email = "invalid@example.com",
                Password = "password123"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _handler.Handle(command, CancellationToken.None);
            });
        }

        [Fact]
        public async Task Handle_InvalidPassword_ShouldThrowException()
        {
            ClearContext(); // Ensure the context is cleared before the test

            // Arrange
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = "wrong-password"
            };

            // Add user with correct password to in-memory DB
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser", // Ensure you set the 'Username'
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "Admin"
            };

            // Add the user only if it's not already in the DB
            if (!_fixture.DbContext.Users.Any(u => u.Id == user.Id))
            {
                _fixture.DbContext.Users.Add(user);
                await _fixture.DbContext.SaveChangesAsync();
            }

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _handler.Handle(command, CancellationToken.None);
            });
        }

        [Fact]
        public async Task Handle_ExistingRefreshToken_ShouldUpdateRefreshToken()
        {
            ClearContext(); // Ensure the context is cleared before the test

            // Arrange
            var command = new LoginCommand
            {
                Email = "test@example.com",
                Password = "password123"
            };

            // Add user and user token to in-memory DB
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                Username = "testuser", // Ensure you set the 'Username'
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = "Admin"
            };

            var oldRefreshToken = "old-refresh-token";
            var userToken = new UserToken
            {
                UserId = 1,
                RefreshToken = oldRefreshToken,
                ExpiryDate = DateTime.UtcNow.AddMonths(1)
            };

            // Add user and token only if not already in DB
            if (!_fixture.DbContext.Users.Any(u => u.Id == user.Id))
            {
                _fixture.DbContext.Users.Add(user);
            }

            if (!_fixture.DbContext.UserTokens.Any(ut => ut.UserId == user.Id))
            {
                _fixture.DbContext.UserTokens.Add(userToken);
            }

            await _fixture.DbContext.SaveChangesAsync();

            // Mock JWT and Refresh Token generation
            _jwtServiceMock.Setup(x => x.GenerateJwtToken(It.IsAny<int>(), It.IsAny<string>())).Returns("jwt-token");
            _tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("new-refresh-token");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("jwt-token", result.Token);
            Assert.Equal("new-refresh-token", result.RefreshToken);

            // Verify that the refresh token was updated in the DB
            var updatedUserToken = await _fixture.DbContext.UserTokens.FirstOrDefaultAsync();
            Assert.Equal("new-refresh-token", updatedUserToken.RefreshToken);
            Assert.NotEqual(oldRefreshToken, updatedUserToken.RefreshToken);
        }
    }


}
