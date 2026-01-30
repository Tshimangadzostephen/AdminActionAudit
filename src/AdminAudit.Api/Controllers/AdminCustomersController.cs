using Microsoft.AspNetCore.Mvc;
using AdminAudit.Api.Data;
using AdminAudit.Api.Models;
using AdminAudit.Domain.Audit;
using Microsoft.EntityFrameworkCore;
namespace AdminAudit.Api.Controllers
{
    [ApiController]
    [Route("admin/customers")]
    public class AdminCustomersController(AppDbContext db, IAuditLogger audit) : ControllerBase
    {
        // Fake actor for demo purposes (in real systems, get from auth/claims)
        private AuditActor Actor => new("admin-123", "admin@example.com");

        [HttpPost("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Status))
                return BadRequest("Status is required");

            var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (customer is null)
            {
                await audit.LogFailureAsync(Actor, "UPDATE_STATUS", new("Customer", id.ToString()), "Customer not found", ct: ct);
                return NotFound();
            }

            var oldStatus = customer.Status;
            customer.Status = req.Status.Trim();

            // Write + audit in one transaction (stronger consistency)
            await using var tx = await db.Database.BeginTransactionAsync(ct);
            await db.SaveChangesAsync(ct);

            await audit.LogSuccessAsync(
                Actor,
                "UPDATE_STATUS",
                new("Customer", id.ToString()),
                changes: new[] { new AuditChange("Status", oldStatus, customer.Status) },
                metadata: new { reason = req.Reason },
                ct: ct
            );

            await tx.CommitAsync(ct);

            return Ok(customer);
        }

        [HttpPost("{id:int}/notes")]
        public async Task<IActionResult> UpdateNotes(int id, [FromBody] UpdateNotesRequest req, CancellationToken ct)
        {
            var customer = await db.Customers.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (customer is null) return NotFound();

            var oldNotes = customer.Notes;
            customer.Notes = req.Notes;

            await db.SaveChangesAsync(ct);

            await audit.LogSuccessAsync(
                Actor,
                "UPDATE_NOTES",
                new("Customer", id.ToString()),
                changes: new[] { new AuditChange("Notes", oldNotes, customer.Notes) },
                ct: ct
            );

            return Ok(customer);
        }

        [HttpPost("seed")]
        public async Task<IActionResult> Seed(CancellationToken ct)
        {
            if (await db.Customers.AnyAsync(ct)) return Ok("Already seeded");

            db.Customers.AddRange(
                new Customer { Name = "Ada Lovelace", Status = "ACTIVE" },
                new Customer { Name = "Alan Turing", Status = "SUSPENDED" }
            );

            await db.SaveChangesAsync(ct);
            return Ok("Seeded");
        }

        public record UpdateStatusRequest(string Status, string? Reason);
        public record UpdateNotesRequest(string? Notes);
    }
}



