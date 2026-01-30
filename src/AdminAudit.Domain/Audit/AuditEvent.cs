using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdminAudit.Domain.Audit
{
    public class AuditEvent
    {
        public long Id { get; set; }
        public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;

        public string ActorUserId { get; set; } = default!;
        public string? ActorEmail { get; set; }

        public string ActionType { get; set; } = default!;
        public string EntityType { get; set; } = default!;
        public string EntityId { get; set; } = default!;

        public string Outcome { get; set; } = AuditOutcomes.Success;
        public string? FailureReason { get; set; }

        public string? CorrelationId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }

        public string? MetadataJson { get; set; }

        public List<AuditEventChange> Changes { get; set; } = new();

        public static string? ToMetadataJson(object? metadata) =>
            metadata is null ? null : JsonSerializer.Serialize(metadata);
    }
}
