using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

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

        if (!await _repo.CategoryExistsAsync(request.CategoryId, ct))
            return new AppResult<ProductDto>.ValidationError("Category does not exist.");

        var normalizedSku = request.Sku.Trim().ToUpperInvariant();
        if (await _repo.ExistsBySkuAsync(normalizedSku, excludeId: id, ct))
            return new AppResult<ProductDto>.Conflict("Product SKU must be unique.");

        entity.Sku = normalizedSku;
        entity.Name = request.Name.Trim();
        entity.Description = request.Description?.Trim();
        entity.CategoryId = request.CategoryId;
        entity.ReorderLevel = request.ReorderLevel;
        entity.IsActive = request.IsActive;
        entity.LastUpdatedUtc = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);
        await _auditLogWriter.WriteAsync("Product", entity.Id.ToString(), "Updated", $"Updated product '{entity.Sku} - {entity.Name}'.", ct);

        var dto = await _repo.GetByIdAsync(id, ct);
        return new AppResult<ProductDto>.Ok(dto!);
    }
}
