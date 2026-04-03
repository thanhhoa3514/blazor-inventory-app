using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Audit.Queries;

public sealed class GetAuditLogByIdQuery
{
    private readonly IAuditLogRepository _repo;

    public GetAuditLogByIdQuery(IAuditLogRepository repo)
    {
        _repo = repo;
    }

    public async Task<AppResult<AuditLogDto>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(id, ct);
        return item is null
            ? new AppResult<AuditLogDto>.NotFound($"Audit log {id} not found.")
            : new AppResult<AuditLogDto>.Ok(item);
    }
}
