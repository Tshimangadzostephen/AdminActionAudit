using AdminAudit.Api.Models;
using AdminAudit.Domain.Audit;
using Microsoft.EntityFrameworkCore;

namespace AdminAudit.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
        public DbSet<AuditEventChange> AuditEventChanges => Set<AuditEventChange>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditEvent>(e =>
            {
                e.HasIndex(x => new { x.ActorUserId, x.OccurredAtUtc });
                e.HasIndex(x => new { x.EntityType, x.EntityId, x.OccurredAtUtc });
                e.Property(x => x.ActorUserId).HasMaxLength(100).IsRequired();
                e.Property(x => x.ActionType).HasMaxLength(80).IsRequired();
                e.Property(x => x.EntityType).HasMaxLength(80).IsRequired();
                e.Property(x => x.EntityId).HasMaxLength(100).IsRequired();
                e.Property(x => x.Outcome).HasMaxLength(20).IsRequired();
                e.Property(x => x.IpAddress).HasMaxLength(45);
                e.Property(x => x.UserAgent).HasMaxLength(256);
                e.Property(x => x.CorrelationId).HasMaxLength(100);
            });

            modelBuilder.Entity<AuditEventChange>(c =>
            {
                c.Property(x => x.FieldName).HasMaxLength(100).IsRequired();
                c.HasIndex(x => x.AuditEventId);
            });

            modelBuilder.Entity<Customer>(c =>
            {
                c.Property(x => x.Name).HasMaxLength(200).IsRequired();
                c.Property(x => x.Status).HasMaxLength(40).IsRequired();
            });
        }
    }
}


