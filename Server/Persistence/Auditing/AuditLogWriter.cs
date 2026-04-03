using MyApp.Server.Application.Common;
using MyApp.Server.Data;
using MyApp.Shared.Domain;

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
            OccurredAtUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync(ct);
    }
}
