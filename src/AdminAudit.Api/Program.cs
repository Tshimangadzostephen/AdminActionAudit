using AdminAudit.Api.Data;
using AdminAudit.Api.Services;
using AdminAudit.Domain.Audit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IHttpContextAuditInfo, HttpContextAuditInfo>();
builder.Services.AddScoped<IAuditLogger, AuditLogger>();

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();

public partial class Program { } // for tests
