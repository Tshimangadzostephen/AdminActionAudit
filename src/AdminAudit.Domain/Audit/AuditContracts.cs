using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminAudit.Domain.Audit
{
    public record AuditActor(string UserId, string? Email);
    public record AuditTarget(string EntityType, string EntityId);
    public record AuditChange(string Field, string? OldValue, string? NewValue);

    public interface IAuditLogger
    {
        Task<long> LogSuccessAsync(
            AuditActor actor,
            string actionType,
            AuditTarget target,
            IEnumerable<AuditChange>? changes = null,
            object? metadata = null,
            CancellationToken ct = default);

        Task<long> LogFailureAsync(
            AuditActor actor,
            string actionType,
            AuditTarget target,
            string failureReason,
            object? metadata = null,
            CancellationToken ct = default);
    }
}
