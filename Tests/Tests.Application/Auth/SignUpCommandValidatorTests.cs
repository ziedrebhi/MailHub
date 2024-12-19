using MailHub.Application.Auth.Commands;
using MailHub.Application.Auth.Validators;

namespace Tests.Application.Auth
{
    public class SignUpCommandValidatorTests
    {
        private readonly SignUpCommandValidator _validator;

        public SignUpCommandValidatorTests()
        {
            _validator = new SignUpCommandValidator();
        }

        [Fact]
        public async Task Validate_ValidCommand_ShouldPassValidation()
        {
            // Arrange
            var command = new SignUpCommand
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password123",
                Role = "User"
            };

            // Act
            var validationResult = await _validator.ValidateAsync(command);

            // Assert
            Assert.True(validationResult.IsValid);
        }

        [Fact]
        public async Task Validate_EmptyUsername_ShouldFailValidation()
        {
            // Arrange
            var command = new SignUpCommand
            {
                Username = "", // Empty username
                Email = "newuser@example.com",
                Password = "Password123",
                Role = "User"
            };

            // Act
            var validationResult = await _validator.ValidateAsync(command);

            // Assert
            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == "Username" && e.ErrorMessage.Contains("Username is required"));
        }

        [Fact]
        public async Task Validate_InvalidEmailFormat_ShouldFailValidation()
        {
            // Arrange
            var command = new SignUpCommand
            {
                Username = "newuser",
                Email = "invalid-email", // Invalid email format
                Password = "Password123",
                Role = "User"
            };

            // Act
            var validationResult = await _validator.ValidateAsync(command);

            // Assert
            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == "Email" && e.ErrorMessage.Contains("Valid email is required"));
        }

        [Fact]
        public async Task Validate_EmptyPassword_ShouldFailValidation()
        {
            // Arrange
            var command = new SignUpCommand
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "", // Empty password
                Role = "User"
            };

            // Act
            var validationResult = await _validator.ValidateAsync(command);

            // Assert
            Assert.False(validationResult.IsValid);

            // Assert that the error is related to the password
            Assert.Contains(validationResult.Errors, e => e.PropertyName == "Password" && e.ErrorMessage.Contains("'Password' must not be empty"));

            // Debugging: Print the errors to see the exact message returned by FluentValidation
            foreach (var error in validationResult.Errors)
            {
                Console.WriteLine($"Error: {error.ErrorMessage}");
            }
        }


        [Fact]
        public async Task Validate_PasswordTooShort_ShouldFailValidation()
        {
            // Arrange
            var command = new SignUpCommand
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "123", // Password too short
                Role = "User"
            };

            // Act
            var validationResult = await _validator.ValidateAsync(command);

            // Assert
            Assert.False(validationResult.IsValid);
            Assert.Contains(validationResult.Errors, e => e.PropertyName == "Password" && e.ErrorMessage.Contains("Password must be at least 6 characters"));
        }
    }
}
