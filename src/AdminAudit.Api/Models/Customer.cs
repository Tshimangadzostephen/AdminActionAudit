namespace AdminAudit.Api.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Status { get; set; } = "ACTIVE";
        public string? Notes { get; set; }
    }
}
