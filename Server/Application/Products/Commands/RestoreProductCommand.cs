using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Products.Commands;

public sealed class RestoreProductCommand
{
    private readonly IProductRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;

    public RestoreProductCommand(IProductRepository repo, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<AppResult<ProductDto>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.FindIncludingDeletedAsync(id, ct);
        if (entity is null)
            return new AppResult<ProductDto>.NotFound($"Product {id} not found.");

        if (!entity.IsDeleted)
            return new AppResult<ProductDto>.Conflict("Product is not deleted.");

        var before = Snapshot(entity);

        entity.IsDeleted = false;
        entity.DeletedAtUtc = null;
        entity.DeletedByUserId = null;
        entity.DeletedByUserName = null;
        entity.LastUpdatedUtc = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);
        await _auditLogWriter.WriteAsync(
            "Product",
            entity.Id.ToString(),
            "Restored",
            $"Restored product '{entity.Sku} - {entity.Name}'.",
            beforeState: before,
            afterState: Snapshot(entity),
            changedFields: new object[]
            {
                new { field = "IsDeleted", oldValue = true, newValue = false },
                new { field = "DeletedByUserName", oldValue = before.DeletedByUserName, newValue = (string?)null }
            },
            ct: ct);

        var dto = await _repo.GetByIdAsync(id, ct);
        return new AppResult<ProductDto>.Ok(dto!);
    }

    private static dynamic Snapshot(Product entity) => new
    {
        entity.Id,
        entity.Sku,
        entity.Name,
        entity.Description,
        entity.CategoryId,
        entity.ReorderLevel,
        entity.IsActive,
        entity.IsDeleted,
        entity.DeletedAtUtc,
        entity.DeletedByUserName
    };
}
