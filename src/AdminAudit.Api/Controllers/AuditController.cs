using Microsoft.AspNetCore.Mvc;
using AdminAudit.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace AdminAudit.Api.Controllers
{
    [ApiController]
    [Route("audit")]
    public class AuditController(AppDbContext db) : ControllerBase
    {
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> ByUser(string userId, [FromQuery] int days = 7, CancellationToken ct = default)
        {
            var since = DateTime.UtcNow.AddDays(-Math.Abs(days));

            var results = await db.AuditEvents
                .Where(x => x.ActorUserId == userId && x.OccurredAtUtc >= since)
                .OrderByDescending(x => x.OccurredAtUtc)
                .Select(x => new {
                    x.Id,
                    x.OccurredAtUtc,
                    x.ActionType,
                    x.EntityType,
                    x.EntityId,
                    x.Outcome,
                    x.CorrelationId
                })
                .Take(200)
                .ToListAsync(ct);

            return Ok(results);
        }

        [HttpGet("entity/{entityType}/{entityId}")]
        public async Task<IActionResult> ByEntity(string entityType, string entityId, CancellationToken ct)
        {
            var results = await db.AuditEvents
                .Where(x => x.EntityType == entityType && x.EntityId == entityId)
                .OrderByDescending(x => x.OccurredAtUtc)
                .Select(x => new {
                    x.Id,
                    x.OccurredAtUtc,
                    x.ActorUserId,
                    x.ActionType,
                    x.Outcome,
                    Changes = x.Changes.Select(c => new { c.FieldName, c.OldValue, c.NewValue })
                })
                .Take(200)
                .ToListAsync(ct);

            return Ok(results);
        }

        [HttpGet("failed")]
        public async Task<IActionResult> Failed([FromQuery] int hours = 24, CancellationToken ct = default)
        {
            var since = DateTime.UtcNow.AddHours(-Math.Abs(hours));

            var results = await db.AuditEvents
                .Where(x => x.Outcome == "FAILED" && x.OccurredAtUtc >= since)
                .OrderByDescending(x => x.OccurredAtUtc)
                .Select(x => new {
                    x.Id,
                    x.OccurredAtUtc,
                    x.ActorUserId,
                    x.ActionType,
                    x.EntityType,
                    x.EntityId,
                    x.FailureReason
                })
                .Take(200)
                .ToListAsync(ct);

            return Ok(results);
        }
    }
}
