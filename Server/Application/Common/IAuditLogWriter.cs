namespace MyApp.Server.Application.Common;

public interface IAuditLogWriter
{
    Task WriteAsync(
        string entityType,
        string entityId,
        string action,
        string summary,
        CancellationToken ct = default);
}
