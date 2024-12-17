using FluentValidation;
using MailHub.Application.EmailConfiguration.Commands;

namespace MailHub.Application.EmailConfiguration.Validators
{
    public class SetEmailConfigurationValidator : AbstractValidator<SetEmailConfigurationCommand>
    {
        public SetEmailConfigurationValidator()
        {
            RuleFor(x => x.SenderEmail).NotEmpty().EmailAddress().WithMessage("Invalid email address.");
            RuleFor(x => x.SenderPassword).NotEmpty().WithMessage("Password is required.");
            RuleFor(x => x.SmtpHost).NotEmpty().WithMessage("SMTP host is required.");
            RuleFor(x => x.SmtpPort).GreaterThan(0).WithMessage("SMTP port must be greater than zero.");
        }
    }
}
