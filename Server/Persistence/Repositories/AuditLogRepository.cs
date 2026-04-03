using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Persistence.Repositories;

public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _db;

    public AuditLogRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<AuditLogDto>> GetAllAsync(
        string? entityType = null,
        string? entityId = null,
        string? actorUserName = null,
        string? action = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken ct = default)
    {
        var query = _db.AuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            var normalized = entityType.Trim();
            query = query.Where(x => x.EntityType == normalized);
        }

        if (!string.IsNullOrWhiteSpace(entityId))
        {
            var normalized = entityId.Trim();
            query = query.Where(x => x.EntityId == normalized);
        }

        if (!string.IsNullOrWhiteSpace(actorUserName))
        {
            var normalized = actorUserName.Trim();
            query = query.Where(x => x.ActorUserName.Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            var normalized = action.Trim();
            query = query.Where(x => x.Action == normalized);
        }

        if (fromUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc <= toUtc.Value);

        return await query
            .OrderByDescending(x => x.OccurredAtUtc)
            .ThenByDescending(x => x.Id)
            .Select(x => new AuditLogDto(
                x.Id,
                x.EntityType,
                x.EntityId,
                x.Action,
                x.ActorUserId,
                x.ActorUserName,
                x.Summary,
                x.OccurredAtUtc))
            .ToListAsync(ct);
    }
}
