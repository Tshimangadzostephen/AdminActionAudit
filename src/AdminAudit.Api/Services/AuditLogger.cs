using AdminAudit.Api.Data;
using AdminAudit.Domain.Audit;
using Microsoft.EntityFrameworkCore;

namespace AdminAudit.Api.Services
{
    public class AuditLogger(AppDbContext db, IHttpContextAuditInfo ctx) : IAuditLogger
    {
        public async Task<long> LogSuccessAsync(
            AuditActor actor,
            string actionType,
            AuditTarget target,
            IEnumerable<AuditChange>? changes = null,
            object? metadata = null,
            CancellationToken ct = default)
        {
            var e = CreateBaseEvent(actor, actionType, target, metadata);
            e.Outcome = AuditOutcomes.Success;

            if (changes is not null)
            {
                foreach (var ch in changes)
                {
                    // Redaction policy hook: block secrets here if needed.
                    e.Changes.Add(new AuditEventChange
                    {
                        FieldName = ch.Field,
                        OldValue = ch.OldValue,
                        NewValue = ch.NewValue
                    });
                }
            }

            db.AuditEvents.Add(e);
            await db.SaveChangesAsync(ct);
            return e.Id;
        }

        public async Task<long> LogFailureAsync(
            AuditActor actor,
            string actionType,
            AuditTarget target,
            string failureReason,
            object? metadata = null,
            CancellationToken ct = default)
        {
            var e = CreateBaseEvent(actor, actionType, target, metadata);
            e.Outcome = AuditOutcomes.Failed;
            e.FailureReason = failureReason;

            db.AuditEvents.Add(e);
            await db.SaveChangesAsync(ct);
            return e.Id;
        }

        private AuditEvent CreateBaseEvent(AuditActor actor, string actionType, AuditTarget target, object? metadata)
        {
            return new AuditEvent
            {
                OccurredAtUtc = DateTime.UtcNow,
                ActorUserId = actor.UserId,
                ActorEmail = actor.Email,
                ActionType = actionType,
                EntityType = target.EntityType,
                EntityId = target.EntityId,
                CorrelationId = ctx.CorrelationId,
                IpAddress = ctx.IpAddress,
                UserAgent = ctx.UserAgent,
                MetadataJson = AuditEvent.ToMetadataJson(metadata)
            };
        }
    }

}


