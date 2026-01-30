using Microsoft.AspNetCore.Http;

namespace AdminAudit.Api.Services
{
    public interface IHttpContextAuditInfo
    {
        string? CorrelationId { get; }
        string? IpAddress { get; }
        string? UserAgent { get; }
    }
    public class HttpContextAuditInfo(IHttpContextAccessor accessor) : IHttpContextAuditInfo
    {
        public string? CorrelationId =>
            accessor.HttpContext?.TraceIdentifier;

        public string? IpAddress =>
            accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        public string? UserAgent =>
            accessor.HttpContext?.Request?.Headers.UserAgent.ToString();
    }
}
