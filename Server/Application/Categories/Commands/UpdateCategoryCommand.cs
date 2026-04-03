using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Categories.Commands;

public sealed class UpdateCategoryCommand
{
    private readonly ICategoryRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;

    public UpdateCategoryCommand(ICategoryRepository repo, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<AppResult<CategoryDto>> ExecuteAsync(int id, UpdateCategoryRequest request, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct);
        if (entity is null)
            return new AppResult<CategoryDto>.NotFound($"Category {id} not found.");

        var before = Snapshot(entity);
        var oldName = entity.Name;
        var oldDescription = entity.Description;

        var normalizedName = request.Name.Trim();
        if (await _repo.ExistsByNameAsync(normalizedName, excludeId: id, ct))
            return new AppResult<CategoryDto>.Conflict("Category name must be unique.");

        entity.Name = normalizedName;
        entity.Description = request.Description?.Trim();

        await _repo.SaveChangesAsync(ct);
        await _auditLogWriter.WriteAsync(
            "Category",
            entity.Id.ToString(),
            "Updated",
            $"Updated category '{entity.Name}'.",
            beforeState: before,
            afterState: Snapshot(entity),
            changedFields: BuildChangedFields(oldName, entity.Name, oldDescription, entity.Description),
            ct: ct);
        return new AppResult<CategoryDto>.Ok(entity.ToDto());
    }

    private static object Snapshot(Category entity) => new
    {
        entity.Id,
        entity.Name,
        entity.Description,
        entity.IsDeleted,
        entity.DeletedAtUtc,
        entity.DeletedByUserName,
        entity.CreatedAtUtc
    };

    private static object[] BuildChangedFields(string oldName, string newName, string? oldDescription, string? newDescription)
    {
        var changed = new List<object>();
        if (!string.Equals(oldName, newName, StringComparison.Ordinal))
            changed.Add(new { field = "Name", oldValue = oldName, newValue = newName });
        if (!string.Equals(oldDescription, newDescription, StringComparison.Ordinal))
            changed.Add(new { field = "Description", oldValue = oldDescription, newValue = newDescription });
        return changed.ToArray();
    }
}
