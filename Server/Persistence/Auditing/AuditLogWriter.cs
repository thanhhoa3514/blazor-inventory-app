using MyApp.Server.Application.Common;
using MyApp.Server.Data;
using MyApp.Shared.Domain;
using System.Text.Json;

namespace MyApp.Server.Persistence.Auditing;

public sealed class AuditLogWriter : IAuditLogWriter
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public AuditLogWriter(AppDbContext db, ICurrentUserAccessor currentUserAccessor)
    {
        _db = db;
        _currentUserAccessor = currentUserAccessor;
    }

    public async Task WriteAsync(
        string entityType,
        string entityId,
        string action,
        string summary,
        object? beforeState = null,
        object? afterState = null,
        object? changedFields = null,
        CancellationToken ct = default)
    {
        var currentUser = _currentUserAccessor.GetRequiredCurrentUser();

        _db.AuditLogs.Add(new AuditLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            ActorUserId = currentUser.UserId,
            ActorUserName = currentUser.UserName,
            Summary = summary,
            BeforeJson = Serialize(beforeState),
            AfterJson = Serialize(afterState),
            ChangedFieldsJson = Serialize(changedFields),
            OccurredAtUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);
    }

    private static string? Serialize(object? value)
    {
        if (value is null)
            return null;

        return JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
    }
}
