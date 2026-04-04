using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Products.Commands;

public sealed class CreateProductCommand
{
    private readonly IProductRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;

    public CreateProductCommand(IProductRepository repo, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<AppResult<ProductDto>> ExecuteAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        if (!await _repo.CategoryExistsAsync(request.CategoryId, ct))
            return new AppResult<ProductDto>.ValidationError("Category does not exist.");

        if (request.TargetStockLevel < request.ReorderLevel)
            return new AppResult<ProductDto>.ValidationError("Target stock level must be greater than or equal to reorder level.");

        if (request.PreferredSupplierId.HasValue && !await _repo.PreferredSupplierExistsAsync(request.PreferredSupplierId.Value, ct))
            return new AppResult<ProductDto>.ValidationError("Preferred supplier does not exist or is inactive.");

        var normalizedSku = request.Sku.Trim().ToUpperInvariant();
        if (await _repo.ExistsBySkuAsync(normalizedSku, excludeId: null, ct))
            return new AppResult<ProductDto>.Conflict("Product SKU must be unique.");

        var entity = new Product
        {
            Sku = normalizedSku,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CategoryId = request.CategoryId,
            PreferredSupplierId = request.PreferredSupplierId,
            ReorderLevel = request.ReorderLevel,
            TargetStockLevel = request.TargetStockLevel,
            OnHandQty = 0,
            AverageCost = 0,
            IsActive = true,
            IsDeleted = false,
            CreatedAtUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        await _auditLogWriter.WriteAsync(
            "Product",
            entity.Id.ToString(),
            "Created",
            $"Created product '{entity.Sku} - {entity.Name}'.",
            afterState: Snapshot(entity),
            changedFields: new object[]
            {
                new { field = "Sku", oldValue = (string?)null, newValue = entity.Sku },
                new { field = "Name", oldValue = (string?)null, newValue = entity.Name },
                new { field = "CategoryId", oldValue = (int?)null, newValue = entity.CategoryId },
                new { field = "PreferredSupplierId", oldValue = (int?)null, newValue = entity.PreferredSupplierId },
                new { field = "ReorderLevel", oldValue = (int?)null, newValue = entity.ReorderLevel },
                new { field = "TargetStockLevel", oldValue = (int?)null, newValue = entity.TargetStockLevel }
            },
            ct: ct);

        var dto = await _repo.GetByIdAsync(entity.Id, ct);
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
}
