using MailHub.Application.Email.Commands;
using MailHub.Application.Email.Handlers;
using MailHub.Application.Interfaces;
using MailHub.Domain.Entities;
using MailHub.Domain.Enums;
using Moq;

namespace Tests.Application.Email
{
    public class SendEmailCommandHandlerTests : IClassFixture<TestDatabaseFixture>
    {
        private readonly TestDatabaseFixture _fixture;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly SendEmailCommandHandler _handler;

        public SendEmailCommandHandlerTests(TestDatabaseFixture fixture)
        {
            _fixture = fixture;
            _emailSenderMock = new Mock<IEmailSender>();
            _handler = new SendEmailCommandHandler(_emailSenderMock.Object);
        }

        private void ClearContext()
        {
            // Clear the change tracker to avoid tracking duplicate entities.
            _fixture.DbContext.ChangeTracker.Clear();
        }

        [Fact]
        public async Task Handle_ValidEmailQueue_ShouldSendEmail()
        {
            ClearContext(); // Ensure the context is cleared before the test

            // Arrange
            var command = new SendEmailCommand
            {
                EmailQueueId = 1
            };

            var emailQueue = new EmailQueue
            {
                Id = 1,
                Status = EmailStatus.Pending,
                Recipient = "recipient@example.com",
                TemplateId = 1,
                Parameters = "{}"
            };

            // Add the EmailQueue to the in-memory DB
            _fixture.DbContext.EmailQueues.Add(emailQueue);
            await _fixture.DbContext.SaveChangesAsync();

            // Mock SendEmailAsync to return true
            _emailSenderMock.Setup(x => x.SendEmailAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _emailSenderMock.Verify(x => x.SendEmailAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_EmailQueueNotFound_ShouldReturnFalse()
        {
            ClearContext(); // Ensure the context is cleared before the test

            // Arrange
            var command = new SendEmailCommand
            {
                EmailQueueId = 999 // This EmailQueueId does not exist in the DB
            };

            // Mock SendEmailAsync to return false
            _emailSenderMock.Setup(x => x.SendEmailAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _emailSenderMock.Verify(x => x.SendEmailAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_EmailSendingFails_ShouldReturnFalse()
        {
            ClearContext(); // Ensure the context is cleared before the test

            // Arrange
            var command = new SendEmailCommand
            {
                EmailQueueId = 2
            };

            var emailQueue = new EmailQueue
            {
                Id = 2,
                Status = EmailStatus.Pending,
                Recipient = "recipient@example.com",
                TemplateId = 1,
                Parameters = "{}"
            };

            // Add the EmailQueue to the in-memory DB
            _fixture.DbContext.EmailQueues.Add(emailQueue);
            await _fixture.DbContext.SaveChangesAsync();

            // Mock SendEmailAsync to return false (simulate failure)
            _emailSenderMock.Setup(x => x.SendEmailAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _emailSenderMock.Verify(x => x.SendEmailAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_CancellationRequested_ShouldNotSendEmail()
        {
            ClearContext(); // Ensure the context is cleared before the test

            // Arrange
            var command = new SendEmailCommand
            {
                EmailQueueId = 3
            };

            var emailQueue = new EmailQueue
            {
                Id = 3,
                Status = EmailStatus.Pending,
                Recipient = "recipient@example.com",
                TemplateId = 1,
                Parameters = "{}"
            };

            // Add the EmailQueue to the in-memory DB
            _fixture.DbContext.EmailQueues.Add(emailQueue);
            await _fixture.DbContext.SaveChangesAsync();

            // Mock SendEmailAsync to simulate that the cancellation token is respected
            _emailSenderMock.Setup(x => x.SendEmailAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                            .ThrowsAsync(new OperationCanceledException());

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await _handler.Handle(command, new CancellationToken(true)); // Pass a canceled token
            });
        }
    }

}
