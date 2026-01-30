using AdminAudit.Api.Data;
using AdminAudit.Api.Services;
using AdminAudit.Domain.Audit;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdminAudit.Tests
{
    public class AuditLoggerTests
    {
        [Fact]
        public async Task LogSuccess_WritesEventAndChanges()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("audit-test-db")
                .Options;

            await using var db = new AppDbContext(options);

            var ctxInfo = new FakeCtx("corr-1", "127.0.0.1", "ua");
            var logger = new AuditLogger(db, ctxInfo);

            var id = await logger.LogSuccessAsync(
                new AuditActor("u1", "u1@mail.com"),
                "UPDATE_STATUS",
                new AuditTarget("Customer", "1"),
                changes: new[] { new AuditChange("Status", "ACTIVE", "SUSPENDED") },
                metadata: new { reason = "test" }
            );

            var saved = await db.AuditEvents.Include(x => x.Changes).FirstAsync(x => x.Id == id);
            Assert.Equal("u1", saved.ActorUserId);
            Assert.Equal("UPDATE_STATUS", saved.ActionType);
            Assert.Single(saved.Changes);
            Assert.Equal("Status", saved.Changes[0].FieldName);
        }

        private class FakeCtx(string? corr, string? ip, string? ua) : IHttpContextAuditInfo
        {
            public string? CorrelationId => corr;
            public string? IpAddress => ip;
            public string? UserAgent => ua;
        }
    }

}



