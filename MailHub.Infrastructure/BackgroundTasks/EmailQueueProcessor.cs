using MailHub.Application.Email.Commands;
using MailHub.Domain.Enums;
using MailHub.Persistence.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MailHub.Infrastructure.BackgroundTasks
{
    public class EmailQueueProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public EmailQueueProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<MailHubDbContext>();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var pendingEmails = await dbContext.EmailQueues
                        .Where(e => e.Status == EmailStatus.Pending)
                        .ToListAsync(stoppingToken);

                    foreach (var email in pendingEmails)
                    {
                        var command = new SendEmailCommand { EmailQueueId = email.Id };
                        await mediator.Send(command, stoppingToken);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Check every 10 seconds
                }
            }
            catch (Exception ex)
            {
                // Log the exception to prevent silent failure
                Console.WriteLine($"BackgroundService failed: {ex.Message}");
            }
        }
    }
}