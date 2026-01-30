using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminAudit.Domain.Audit
{
    public class AuditEventChange
    {
        public long Id { get; set; }

        public long AuditEventId { get; set; }
        public AuditEvent AuditEvent { get; set; } = default!;

        public string FieldName { get; set; } = default!;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
