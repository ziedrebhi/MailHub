using MailHub.Application.Auth.Commands;
using MailHub.Application.Auth.Handlers;
using MailHub.Application.Auth.Validators;
using MailHub.Domain.Entities;

namespace Tests.Application.Auth
{
    public class SignUpCommandHandlerTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _fixture;
        private readonly SignUpCommandHandler _handler;

        public SignUpCommandHandlerTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
            _handler = new SignUpCommandHandler(_fixture.DbContext);
        }

        private void ClearContext()
        {
            // Clear the change tracker to avoid tracking duplicate entities.
            _fixture.DbContext.ChangeTracker.Clear();
        }

        [Fact]
        public async Task Handle_ValidSignUp_ShouldCreateNewUser()
        {
            ClearContext(); // Ensure the context is cleared before the test

            // Arrange
            var command = new SignUpCommand
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password123",
                Role = "User"
            };

            // Act
            var userId = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(userId > 0);
            var user = await _fixture.DbContext.Users.FindAsync(userId);
            Assert.NotNull(user);
            Assert.Equal("newuser", user.Username);
            Assert.Equal("newuser@example.com", user.Email);
        }

        [Fact]
        public async Task Handle_DuplicateEmail_ShouldThrowException()
        {
            ClearContext(); // Ensure the context is cleared before the test

            // Arrange
            var existingUser = new User
            {
                Username = "existinguser",
                Email = "existinguser@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _fixture.DbContext.Users.Add(existingUser);
            await _fixture.DbContext.SaveChangesAsync();

            var command = new SignUpCommand
            {
                Username = "newuser",
                Email = "existinguser@example.com", // Same email as existing user
                Password = "Password123",
                Role = "User"
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _handler.Handle(command, CancellationToken.None);
            });
        }

        [Fact]
        public async Task Handle_InvalidEmailFormat_ShouldFailValidation()
        {
            ClearContext(); // Ensure the context is cleared before the test

            // Arrange
            var command = new SignUpCommand
            {
                Username = "newuser",
                Email = "invalid-email", // Invalid email format
                Password = "Password123",
                Role = "User"
            };

            // Act & Assert
            var validator = new SignUpCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == "Email" && e.ErrorMessage.Contains("Valid email is required"));
        }

        [Fact]
        public async Task Handle_EmptyUsername_ShouldFailValidation()
        {
            ClearContext(); // Ensure the context is cleared before the test

            // Arrange
            var command = new SignUpCommand
            {
                Username = "", // Empty username
                Email = "newuser@example.com",
                Password = "Password123",
                Role = "User"
            };

            // Act & Assert
            var validator = new SignUpCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == "Username" && e.ErrorMessage.Contains("Username is required"));
        }

        [Fact]
        public async Task Handle_ShortPassword_ShouldFailValidation()
        {
            ClearContext(); // Ensure the context is cleared before the test

            // Arrange
            var command = new SignUpCommand
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "123", // Password too short
                Role = "User"
            };

            // Act & Assert
            var validator = new SignUpCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == "Password" && e.ErrorMessage.Contains("Password must be at least 6 characters"));
        }
    }

}
