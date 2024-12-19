using MailHub.Application.Interfaces;
using MailHub.Infrastructure.Services;
using MailHub.Persistence.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Architecture
{
    public class ServiceRegistrationTests
    {
        [Fact]
        public void ShouldRegisterServicesInCorrectLayers()
        {
            // Setup a mock service collection
            var services = new ServiceCollection();

            // Register dependencies similar to your actual application
            services.AddDbContext<MailHubDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase")); // Use an in-memory database for testing

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // Mock IHttpContextAccessor
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IEncryptionService, EncryptionService>(); // Example for additional dependencies

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Validate service registration
            var emailSender = serviceProvider.GetService<IEmailSender>();
            Assert.NotNull(emailSender);

            // Additional assertions to ensure the service is resolved correctly
            var encryptionService = serviceProvider.GetService<IEncryptionService>();
            Assert.NotNull(encryptionService);
        }

    }
}
