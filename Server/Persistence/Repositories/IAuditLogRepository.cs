using MyApp.Shared.Contracts;

namespace MyApp.Server.Persistence.Repositories;

public interface IAuditLogRepository
{
    Task<IReadOnlyList<AuditLogDto>> GetAllAsync(
        string? entityType = null,
        string? entityId = null,
        string? actorUserName = null,
        string? action = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken ct = default);
}
