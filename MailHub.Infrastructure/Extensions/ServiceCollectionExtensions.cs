using MailHub.Application.Interfaces;
using MailHub.Infrastructure.BackgroundTasks;
using MailHub.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MailHub.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {

            services.AddHostedService<EmailQueueProcessor>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddSingleton<IJWTService, JWTService>();
            services.AddSingleton<IEncryptionService, EncryptionService>();
            return services;
        }
    }
}
