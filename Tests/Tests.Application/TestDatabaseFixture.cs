using MailHub.Persistence.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Application
{
    public class TestDatabaseFixture : IDisposable
    {
        public DbContextOptions<MailHubDbContext> Options { get; private set; }
        public MailHubDbContext DbContext { get; private set; }

        public TestDatabaseFixture()
        {
            // Using a unique name for each test to avoid sharing the same DB instance.
            var databaseName = Guid.NewGuid().ToString();
            Options = new DbContextOptionsBuilder<MailHubDbContext>()
                         .UseInMemoryDatabase(databaseName)  // Unique DB name
                         .Options;

            DbContext = new MailHubDbContext(Options, Mock.Of<IHttpContextAccessor>());
            DbContext.Database.EnsureCreated(); // Ensure the database schema is created
        }

        public void Dispose()
        {
            // Clean up resources
            DbContext.Database.EnsureDeleted(); // Delete the database after the test run
            DbContext?.Dispose();
        }
    }
}
