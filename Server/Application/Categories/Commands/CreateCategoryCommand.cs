using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Categories.Commands;

public sealed class CreateCategoryCommand
{
    private readonly ICategoryRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;

    public CreateCategoryCommand(ICategoryRepository repo, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<AppResult<CategoryDto>> ExecuteAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        var normalizedName = request.Name.Trim();

        if (await _repo.ExistsByNameAsync(normalizedName, excludeId: null, ct))
            return new AppResult<CategoryDto>.Conflict("Category name must be unique.");

        var entity = new Category
        {
            Name = normalizedName,
            Description = request.Description?.Trim(),
            IsDeleted = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        await _auditLogWriter.WriteAsync(
            "Category",
            entity.Id.ToString(),
            "Created",
            $"Created category '{entity.Name}'.",
            afterState: Snapshot(entity),
            changedFields: new object[]
            {
                new { field = "Name", oldValue = (string?)null, newValue = entity.Name },
                new { field = "Description", oldValue = (string?)null, newValue = entity.Description }
            },
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
}
