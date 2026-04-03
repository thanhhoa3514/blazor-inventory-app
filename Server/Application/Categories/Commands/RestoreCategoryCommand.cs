using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Categories.Commands;

public sealed class RestoreCategoryCommand
{
    private readonly ICategoryRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;

    public RestoreCategoryCommand(ICategoryRepository repo, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<AppResult<CategoryDto>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.FindIncludingDeletedAsync(id, ct);
        if (entity is null)
            return new AppResult<CategoryDto>.NotFound($"Category {id} not found.");

        if (!entity.IsDeleted)
            return new AppResult<CategoryDto>.Conflict("Category is not deleted.");

        var before = Snapshot(entity);

        entity.IsDeleted = false;
        entity.DeletedAtUtc = null;
        entity.DeletedByUserId = null;
        entity.DeletedByUserName = null;

        await _repo.SaveChangesAsync(ct);
        await _auditLogWriter.WriteAsync(
            "Category",
            entity.Id.ToString(),
            "Restored",
            $"Restored category '{entity.Name}'.",
            beforeState: before,
            afterState: Snapshot(entity),
            changedFields: new object[]
            {
                new { field = "IsDeleted", oldValue = true, newValue = false },
                new { field = "DeletedByUserName", oldValue = before.DeletedByUserName, newValue = (string?)null }
            },
            ct: ct);

        var dto = await _repo.GetByIdAsync(id, ct);
        return new AppResult<CategoryDto>.Ok(dto!);
    }

    private static dynamic Snapshot(Category entity) => new
    {
        entity.Id,
        entity.Name,
        entity.Description,
        entity.IsDeleted,
        entity.DeletedAtUtc,
        entity.DeletedByUserName
    };
}
