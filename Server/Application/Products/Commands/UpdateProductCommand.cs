using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Products.Commands;

public sealed class UpdateProductCommand
{
    private readonly IProductRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;

    public UpdateProductCommand(IProductRepository repo, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<AppResult<ProductDto>> ExecuteAsync(int id, UpdateProductRequest request, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct);
        if (entity is null)
            return new AppResult<ProductDto>.NotFound($"Product {id} not found.");

        var before = Snapshot(entity);
        var oldSku = entity.Sku;
        var oldName = entity.Name;
        var oldDescription = entity.Description;
        var oldCategoryId = entity.CategoryId;
        var oldPreferredSupplierId = entity.PreferredSupplierId;
        var oldReorderLevel = entity.ReorderLevel;
        var oldTargetStockLevel = entity.TargetStockLevel;
        var oldIsActive = entity.IsActive;

        if (!await _repo.CategoryExistsAsync(request.CategoryId, ct))
            return new AppResult<ProductDto>.ValidationError("Category does not exist.");

        if (request.TargetStockLevel < request.ReorderLevel)
            return new AppResult<ProductDto>.ValidationError("Target stock level must be greater than or equal to reorder level.");

        if (request.PreferredSupplierId.HasValue && !await _repo.PreferredSupplierExistsAsync(request.PreferredSupplierId.Value, ct))
            return new AppResult<ProductDto>.ValidationError("Preferred supplier does not exist or is inactive.");

        var normalizedSku = request.Sku.Trim().ToUpperInvariant();
        if (await _repo.ExistsBySkuAsync(normalizedSku, excludeId: id, ct))
            return new AppResult<ProductDto>.Conflict("Product SKU must be unique.");

        entity.Sku = normalizedSku;
        entity.Name = request.Name.Trim();
        entity.Description = request.Description?.Trim();
        entity.CategoryId = request.CategoryId;
        entity.PreferredSupplierId = request.PreferredSupplierId;
        entity.ReorderLevel = request.ReorderLevel;
        entity.TargetStockLevel = request.TargetStockLevel;
        entity.IsActive = request.IsActive;
        entity.LastUpdatedUtc = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);
        await _auditLogWriter.WriteAsync(
            "Product",
            entity.Id.ToString(),
            "Updated",
            $"Updated product '{entity.Sku} - {entity.Name}'.",
            beforeState: before,
            afterState: Snapshot(entity),
            changedFields: BuildChangedFields(oldSku, oldName, oldDescription, oldCategoryId, oldPreferredSupplierId, oldReorderLevel, oldTargetStockLevel, oldIsActive, entity),
            ct: ct);

        var dto = await _repo.GetByIdAsync(id, ct);
        return new AppResult<ProductDto>.Ok(dto!);
    }

    private static object Snapshot(Product entity) => new
    {
        entity.Id,
        entity.Sku,
        entity.Name,
        entity.Description,
        entity.CategoryId,
        entity.PreferredSupplierId,
        entity.OnHandQty,
        entity.AverageCost,
        entity.ReorderLevel,
        entity.TargetStockLevel,
        entity.IsActive,
        entity.IsDeleted,
        entity.DeletedAtUtc,
        entity.DeletedByUserName,
        entity.LastUpdatedUtc
    };

    private static object[] BuildChangedFields(
        string oldSku,
        string oldName,
        string? oldDescription,
        int oldCategoryId,
        int? oldPreferredSupplierId,
        int oldReorderLevel,
        int oldTargetStockLevel,
        bool oldIsActive,
        Product entity)
    {
        var changed = new List<object>();
        if (!string.Equals(oldSku, entity.Sku, StringComparison.Ordinal))
            changed.Add(new { field = "Sku", oldValue = oldSku, newValue = entity.Sku });
        if (!string.Equals(oldName, entity.Name, StringComparison.Ordinal))
            changed.Add(new { field = "Name", oldValue = oldName, newValue = entity.Name });
        if (!string.Equals(oldDescription, entity.Description, StringComparison.Ordinal))
            changed.Add(new { field = "Description", oldValue = oldDescription, newValue = entity.Description });
        if (oldCategoryId != entity.CategoryId)
            changed.Add(new { field = "CategoryId", oldValue = oldCategoryId, newValue = entity.CategoryId });
        if (oldPreferredSupplierId != entity.PreferredSupplierId)
            changed.Add(new { field = "PreferredSupplierId", oldValue = oldPreferredSupplierId, newValue = entity.PreferredSupplierId });
        if (oldReorderLevel != entity.ReorderLevel)
            changed.Add(new { field = "ReorderLevel", oldValue = oldReorderLevel, newValue = entity.ReorderLevel });
        if (oldTargetStockLevel != entity.TargetStockLevel)
            changed.Add(new { field = "TargetStockLevel", oldValue = oldTargetStockLevel, newValue = entity.TargetStockLevel });
        if (oldIsActive != entity.IsActive)
            changed.Add(new { field = "IsActive", oldValue = oldIsActive, newValue = entity.IsActive });
        return changed.ToArray();
    }
}
