\# Admin Action Audit



A lightweight audit logging system for an internal admin application.  

This solution captures \*\*who performed an action, what action occurred, which record was affected, and when it happened\*\*, with optional field-level change tracking to support investigation and traceability.



The system is designed for operational insight and debugging, not regulatory compliance.



---



\## Tech Stack



\- ASP.NET Core Web API (.NET)

\- Entity Framework Core

\- SQL Server (LocalDB / SQL Express)

\- Swagger UI for API testing

\- xUnit for testing



---



\## Key Features



\- Append-only audit event logging

\- Tracks:

&nbsp; - Actor (User ID / Email)

&nbsp; - Action type

&nbsp; - Target entity and record

&nbsp; - Timestamp

&nbsp; - Outcome (success/failure)

&nbsp; - Request metadata

\- Optional field-level change tracking

\- Query endpoints for investigation

\- Clean separation of concerns

\- Unit and smoke tests included



---



\## Project Structure



src/

&nbsp; AdminAudit.Api/        # Web API layer

&nbsp; AdminAudit.Domain/     # Audit domain models

&nbsp; AdminAudit.Tests/      # Unit and integration tests



---



\## Audit Event Model



Each audit event records:



\- ActorUserId

\- ActorEmail

\- ActionType

\- EntityType

\- EntityId

\- OccurredAtUtc

\- Outcome

\- FailureReason (if applicable)

\- CorrelationId

\- IpAddress

\- UserAgent

\- MetadataJson



Optional:

\- Field-level changes stored in AuditEventChange



---



\## Database Schema



\### AuditEvent

\- Id (Primary Key)

\- OccurredAtUtc

\- ActorUserId

\- ActorEmail

\- ActionType

\- EntityType

\- EntityId

\- Outcome

\- FailureReason

\- CorrelationId

\- IpAddress

\- UserAgent

\- MetadataJson



\### AuditEventChange

\- Id (Primary Key)

\- AuditEventId (Foreign Key)

\- FieldName

\- OldValue

\- NewValue



Indexes:

\- ActorUserId + OccurredAtUtc

\- EntityType + EntityId + OccurredAtUtc



Sensitive information is excluded from audit logs.



---



\## Example Queries



\### What actions did user X perform last week?



SELECT occurred\_at\_utc, action\_type, entity\_type, entity\_id

FROM AuditEvent

WHERE actor\_user\_id = @UserId

AND occurred\_at\_utc >= DATEADD(DAY, -7, SYSUTCDATETIME())

ORDER BY occurred\_at\_utc DESC;



\### What changed on record Y?



SELECT e.occurred\_at\_utc, e.actor\_user\_id, c.field\_name, c.old\_value, c.new\_value

FROM AuditEvent e

JOIN AuditEventChange c ON e.id = c.audit\_event\_id

WHERE e.entity\_type = @EntityType

AND e.entity\_id = @EntityId

ORDER BY e.occurred\_at\_utc DESC;



---



\## Getting Started



\### Prerequisites



\- .NET SDK installed

\- Local SQL Server (LocalDB or SQL Express)



---



\## Configuration



Update the connection string in:



src/AdminAudit.Api/appsettings.json



{

&nbsp; "ConnectionStrings": {

&nbsp;   "Default": "Server=(localdb)\\\\MSSQLLocalDB;Database=AdminAuditDb;Trusted\_Connection=True;TrustServerCertificate=True"

&nbsp; }

}



If using SQL Express:



"Server=.\\\\SQLEXPRESS;Database=AdminAuditDb;Trusted\_Connection=True;TrustServerCertificate=True"



---



\## Build and Run



\### Restore dependencies

dotnet restore



\### Build the project

dotnet build



\### Run the API

dotnet run --project src/AdminAudit.Api



---



\## Swagger UI



Once running, open:



https://localhost:<port>/swagger



Swagger provides a UI to test endpoints.



---



\## Database Setup (EF Core)



Install EF Core tool (if needed):



dotnet tool install --global dotnet-ef



Create migration:



dotnet ef migrations add InitialCreate -p src/AdminAudit.Api -s src/AdminAudit.Api



Apply migration:



dotnet ef database update -p src/AdminAudit.Api -s src/AdminAudit.Api



---



\## Example Usage



Seed demo data:

POST /admin/customers/seed



Update customer status:

POST /admin/customers/{id}/status



Query user actions:

GET /audit/user/{userId}?days=7



Query changes on a record:

GET /audit/entity/{entityType}/{entityId}



---



\## Testing



Run tests:



dotnet test



Tests cover:

\- Audit logging accuracy

\- Change tracking

\- API startup validation



---



\## Design Trade-offs



\- Not a compliance-grade audit trail

\- No cryptographic verification

\- No immutable storage

\- Storage grows over time; retention policy required in production

\- Field-level tracking increases data volume



---



\## Limitations



\- Designed for moderate internal usage

\- Not optimized for high-scale logging

\- No automated archiving or retention policies implemented



---



\## Summary



This solution demonstrates:

\- Practical audit logging design

\- Clean architecture separation

\- Realistic trade-off awareness

\- Query-driven data modeling

\- Testing and validation strategies



