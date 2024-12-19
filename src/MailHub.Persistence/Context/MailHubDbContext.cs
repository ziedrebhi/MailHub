using MailHub.Domain.Common;
using MailHub.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MailHub.Persistence.Context
{
    public class MailHubDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MailHubDbContext(DbContextOptions<MailHubDbContext> options, IHttpContextAccessor httpContextAccessor)
        : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailQueue> EmailQueues { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<EmailConfiguration> EmailConfigurations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Set the default schema for all entities to "MailHub"
            modelBuilder.HasDefaultSchema("MailHub");
            // Shared Configuration for BaseEntity (CreatedBy, UpdatedBy)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property("CreatedBy")
                        .IsRequired();

                    modelBuilder.Entity(entityType.ClrType)
                        .Property("UpdatedBy")
                        .IsRequired();
                }
            }

            // EmailTemplate Configuration
            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Subject).IsRequired();
                entity.Property(e => e.Body).IsRequired();
                entity.Property(e => e.Parameters).HasConversion(
                    v => string.Join(",", v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );
            });

            // EmailQueue Configuration
            modelBuilder.Entity<EmailQueue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Recipient).IsRequired();
                entity.Property(e => e.Parameters).IsRequired();
                entity.HasOne(e => e.Template)
                      .WithMany()
                      .HasForeignKey(e => e.TemplateId);
            });

            // EmailLog Configuration
            modelBuilder.Entity<EmailLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.EmailQueue)
                      .WithMany()
                      .HasForeignKey(e => e.EmailQueueId);
            });

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).IsRequired();
            });

            // UserToken Configuration
            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.HasKey(ut => ut.Id);
                entity.Property(ut => ut.RefreshToken).IsRequired();
                entity.Property(ut => ut.ExpiryDate).IsRequired();
                entity.HasOne(ut => ut.User)
                      .WithMany()
                      .HasForeignKey(ut => ut.UserId);
            });


            base.OnModelCreating(modelBuilder);
        }
        private int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;  // Default to 0 or another value if not found
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                var userId = GetCurrentUserId();

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = userId;  // Set created by user
                }

                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedBy = userId;  // Set updated by user
            }

            return await base.SaveChangesAsync(cancellationToken);
        }


    }
}
