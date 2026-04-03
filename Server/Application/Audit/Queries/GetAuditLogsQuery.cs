using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Audit.Queries;

public sealed class GetAuditLogsQuery
{
    private readonly IAuditLogRepository _repo;

    public GetAuditLogsQuery(IAuditLogRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<AuditLogDto>> ExecuteAsync(
        string? entityType,
        string? entityId,
        string? actorUserName,
        string? action,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken ct = default)
        => _repo.GetAllAsync(entityType, entityId, actorUserName, action, fromUtc, toUtc, ct);
}
