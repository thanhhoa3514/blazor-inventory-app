using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;

namespace MyApp.Server.Application.Categories.Commands;

public sealed class DeleteCategoryCommand
{
    private readonly ICategoryRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;

    public DeleteCategoryCommand(ICategoryRepository repo, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<AppResult<Unit>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct);
        if (entity is null)
            return new AppResult<Unit>.NotFound($"Category {id} not found.");

        if (await _repo.HasProductsAsync(id, ct))
            return new AppResult<Unit>.Conflict("Cannot delete category while products exist.");

        await _repo.DeleteAsync(entity, ct);
        await _auditLogWriter.WriteAsync("Category", id.ToString(), "Deleted", $"Deleted category '{entity.Name}'.", ct);
        return new AppResult<Unit>.Ok(Unit.Value);
    }
}
