using FluentAssertions;
using MailHub.Application.Auth.Commands;
using MailHub.Application.Auth.Validators;

namespace Tests.Application.Auth
{
    public class LoginCommandValidatorTests
    {
        private readonly LoginCommandValidator _validator;

        public LoginCommandValidatorTests()
        {
            _validator = new LoginCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var command = new LoginCommand { Email = "", Password = "password123" };
            var result = _validator.Validate(command);

            result.Errors.Should().ContainSingle(f => f.PropertyName == "Email" && f.ErrorMessage == "Valid email is required");
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var command = new LoginCommand { Email = "invalidemail", Password = "password123" };
            var result = _validator.Validate(command);

            result.Errors.Should().ContainSingle(f => f.PropertyName == "Email" && f.ErrorMessage == "Valid email is required");
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            var command = new LoginCommand { Email = "user@example.com", Password = "" };
            var result = _validator.Validate(command);

            result.Errors.Should().ContainSingle(f => f.PropertyName == "Password" && f.ErrorMessage == "Password is required");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Valid_Email_And_Password()
        {
            var command = new LoginCommand { Email = "user@example.com", Password = "password123" };
            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }

}
