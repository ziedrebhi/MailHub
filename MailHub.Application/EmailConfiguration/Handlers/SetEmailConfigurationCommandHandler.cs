using MailHub.Application.EmailConfiguration.Commands;
using MailHub.Application.Interfaces;
using MailHub.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MailHub.Application.EmailConfiguration.Handlers
{
    public class SetEmailConfigurationCommandHandler : IRequestHandler<SetEmailConfigurationCommand, bool>
    {
        private readonly MailHubDbContext _dbContext;
        private readonly IEncryptionService _encryptionService;

        public SetEmailConfigurationCommandHandler(MailHubDbContext dbContext, IEncryptionService encryptionService)
        {
            _dbContext = dbContext;
            _encryptionService = encryptionService;
        }

        public async Task<bool> Handle(SetEmailConfigurationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.IsDefault)
                {
                    // Unset the previous default configuration
                    var existingDefaultConfig = await _dbContext.EmailConfigurations
                        .Where(c => c.IsDefault)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (existingDefaultConfig != null)
                    {
                        existingDefaultConfig.IsDefault = false;
                        _dbContext.EmailConfigurations.Update(existingDefaultConfig);
                    }
                }

                // Check if updating an existing configuration
                if (request.Id.HasValue)
                {
                    var existingConfig = await _dbContext.EmailConfigurations
                        .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                    if (existingConfig != null)
                    {
                        existingConfig.SenderEmail = request.SenderEmail;
                        existingConfig.SenderPassword = _encryptionService.Encrypt(request.SenderPassword);  // Encrypt password
                        existingConfig.SmtpHost = request.SmtpHost;
                        existingConfig.SmtpPort = request.SmtpPort;
                        existingConfig.EnableSsl = request.EnableSsl;
                        existingConfig.IsDefault = request.IsDefault;

                        _dbContext.EmailConfigurations.Update(existingConfig);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    // Create a new configuration
                    var newConfig = new Domain.Entities.EmailConfiguration
                    {
                        SenderEmail = request.SenderEmail,
                        SenderPassword = _encryptionService.Encrypt(request.SenderPassword),  // Encrypt password
                        SmtpHost = request.SmtpHost,
                        SmtpPort = request.SmtpPort,
                        EnableSsl = request.EnableSsl,
                        IsDefault = request.IsDefault
                    };
                    await _dbContext.EmailConfigurations.AddAsync(newConfig, cancellationToken);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}