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

        var normalizedSku = request.Sku.Trim().ToUpperInvariant();
        if (await _repo.ExistsBySkuAsync(normalizedSku, excludeId: null, ct))
            return new AppResult<ProductDto>.Conflict("Product SKU must be unique.");

        var entity = new Product
        {
            Sku = normalizedSku,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CategoryId = request.CategoryId,
            ReorderLevel = request.ReorderLevel,
            OnHandQty = 0,
            AverageCost = 0,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        await _auditLogWriter.WriteAsync("Product", entity.Id.ToString(), "Created", $"Created product '{entity.Sku} - {entity.Name}'.", ct);

        var dto = await _repo.GetByIdAsync(entity.Id, ct);
        return new AppResult<ProductDto>.Ok(dto!);
    }
}
