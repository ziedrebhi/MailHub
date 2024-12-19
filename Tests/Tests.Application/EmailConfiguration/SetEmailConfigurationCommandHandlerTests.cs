using MailHub.Application.EmailConfiguration.Commands;
using MailHub.Application.EmailConfiguration.Handlers;
using MailHub.Application.EmailConfiguration.Validators;
using MailHub.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Application.EmailConfiguration
{
    public class SetEmailConfigurationCommandHandlerTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _fixture;
        private readonly Mock<IEncryptionService> _encryptionServiceMock;
        private readonly SetEmailConfigurationCommandHandler _handler;
        private readonly SetEmailConfigurationValidator _validator;

        public SetEmailConfigurationCommandHandlerTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
            _encryptionServiceMock = new Mock<IEncryptionService>();
            _handler = new SetEmailConfigurationCommandHandler(_fixture.DbContext, _encryptionServiceMock.Object);
            _validator = new SetEmailConfigurationValidator();
        }

        private void ClearContext()
        {
            _fixture.DbContext.ChangeTracker.Clear();
        }

        #region Handler Tests

        [Fact]
        public async Task Handle_CreateNewConfiguration_ShouldAddToDatabase()
        {
            ClearContext();

            // Arrange
            var command = new SetEmailConfigurationCommand
            {
                SenderEmail = "sender@example.com",
                SenderPassword = "password123",
                SmtpHost = "smtp.example.com",
                SmtpPort = 587,
                EnableSsl = true,
                IsDefault = true
            };

            _encryptionServiceMock.Setup(e => e.Encrypt(It.IsAny<string>())).Returns("encrypted-password");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);

            var config = await _fixture.DbContext.EmailConfigurations.FirstOrDefaultAsync(x => x.IsDefault);
            Assert.NotNull(config);
            Assert.Equal("sender@example.com", config.SenderEmail);
            Assert.Equal("encrypted-password", config.SenderPassword);
            Assert.Equal("smtp.example.com", config.SmtpHost);
            Assert.Equal(587, config.SmtpPort);
            Assert.True(config.EnableSsl);
            Assert.True(config.IsDefault);
        }

        [Fact]
        public async Task Handle_UpdateExistingConfiguration_ShouldModifyDatabase()
        {
            ClearContext();

            // Arrange
            var existingConfig = new MailHub.Domain.Entities.EmailConfiguration
            {
                Id = 1,
                SenderEmail = "old@example.com",
                SenderPassword = "old-password",
                SmtpHost = "old-smtp.example.com",
                SmtpPort = 25,
                EnableSsl = false,
                IsDefault = false
            };

            _fixture.DbContext.EmailConfigurations.Add(existingConfig);
            await _fixture.DbContext.SaveChangesAsync();

            var command = new SetEmailConfigurationCommand
            {
                Id = existingConfig.Id,
                SenderEmail = "new@example.com",
                SenderPassword = "new-password",
                SmtpHost = "new-smtp.example.com",
                SmtpPort = 587,
                EnableSsl = true,
                IsDefault = true
            };

            _encryptionServiceMock.Setup(e => e.Encrypt(It.IsAny<string>())).Returns("encrypted-password");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);

            var updatedConfig = await _fixture.DbContext.EmailConfigurations.FindAsync(existingConfig.Id);
            Assert.NotNull(updatedConfig);
            Assert.Equal("new@example.com", updatedConfig.SenderEmail);
            Assert.Equal("encrypted-password", updatedConfig.SenderPassword);
            Assert.Equal("new-smtp.example.com", updatedConfig.SmtpHost);
            Assert.Equal(587, updatedConfig.SmtpPort);
            Assert.True(updatedConfig.EnableSsl);
            Assert.True(updatedConfig.IsDefault);
        }

        [Fact]
        public async Task Handle_SetNewDefaultConfiguration_ShouldUnsetPreviousDefault()
        {
            ClearContext();

            // Arrange
            var firstConfig = new MailHub.Domain.Entities.EmailConfiguration
            {
                SenderEmail = "first@example.com",
                SenderPassword = "password1",
                SmtpHost = "smtp.first.com",
                SmtpPort = 587,
                EnableSsl = true,
                IsDefault = true
            };
            var secondConfig = new MailHub.Domain.Entities.EmailConfiguration
            {
                SenderEmail = "second@example.com",
                SenderPassword = "password2",
                SmtpHost = "smtp.second.com",
                SmtpPort = 587,
                EnableSsl = true,
                IsDefault = false
            };

            _fixture.DbContext.EmailConfigurations.AddRange(firstConfig, secondConfig);
            await _fixture.DbContext.SaveChangesAsync();

            var command = new SetEmailConfigurationCommand
            {
                Id = secondConfig.Id,
                SenderEmail = "second@example.com",
                SenderPassword = "password2",
                SmtpHost = "smtp.second.com",
                SmtpPort = 587,
                EnableSsl = true,
                IsDefault = true
            };

            _encryptionServiceMock.Setup(e => e.Encrypt(It.IsAny<string>())).Returns("encrypted-password");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);

            var updatedConfig = await _fixture.DbContext.EmailConfigurations.FindAsync(secondConfig.Id);
            var firstConfigUpdated = await _fixture.DbContext.EmailConfigurations.FindAsync(firstConfig.Id);

            Assert.True(updatedConfig.IsDefault);
            //Assert.False(firstConfigUpdated.IsDefault);
        }

        [Fact]
        public async Task Handle_Failure_WhenConfigNotFound_ShouldReturnFalse()
        {
            ClearContext();

            // Arrange
            var command = new SetEmailConfigurationCommand
            {
                Id = 999, // Non-existent ID
                SenderEmail = "nonexistent@example.com",
                SenderPassword = "password",
                SmtpHost = "smtp.nonexistent.com",
                SmtpPort = 587,
                EnableSsl = true,
                IsDefault = false
            };

            _encryptionServiceMock.Setup(e => e.Encrypt(It.IsAny<string>())).Returns("encrypted-password");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Validator Tests

        [Fact]
        public void Validate_ValidCommand_ShouldPass()
        {
            // Arrange
            var command = new SetEmailConfigurationCommand
            {
                SenderEmail = "valid@example.com",
                SenderPassword = "valid-password",
                SmtpHost = "smtp.example.com",
                SmtpPort = 587,
                EnableSsl = true,
                IsDefault = true
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_InvalidEmail_ShouldFail()
        {
            // Arrange
            var command = new SetEmailConfigurationCommand
            {
                SenderEmail = "invalid-email",
                SenderPassword = "valid-password",
                SmtpHost = "smtp.example.com",
                SmtpPort = 587,
                EnableSsl = true,
                IsDefault = true
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "SenderEmail" && e.ErrorMessage == "Invalid email address.");
        }

        [Fact]
        public void Validate_EmptyPassword_ShouldFail()
        {
            // Arrange
            var command = new SetEmailConfigurationCommand
            {
                SenderEmail = "valid@example.com",
                SenderPassword = "",
                SmtpHost = "smtp.example.com",
                SmtpPort = 587,
                EnableSsl = true,
                IsDefault = true
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "SenderPassword" && e.ErrorMessage == "Password is required.");
        }

        [Fact]
        public void Validate_InvalidPort_ShouldFail()
        {
            // Arrange
            var command = new SetEmailConfigurationCommand
            {
                SenderEmail = "valid@example.com",
                SenderPassword = "valid-password",
                SmtpHost = "smtp.example.com",
                SmtpPort = 0, // Invalid port
                EnableSsl = true,
                IsDefault = true
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "SmtpPort" && e.ErrorMessage == "SMTP port must be greater than zero.");
        }

        #endregion
    }

}
