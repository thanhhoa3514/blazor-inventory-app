using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Common;

public static class ProductMapper
{
    public static ProductDto ToDto(this Product entity, string categoryName)
        => new(
            entity.Id,
            entity.Sku,
            entity.Name,
            entity.Description,
            entity.CategoryId,
            categoryName,
            entity.PreferredSupplierId,
            entity.PreferredSupplier?.Name,
            entity.OnHandQty,
            entity.AverageCost,
            entity.ReorderLevel,
            entity.TargetStockLevel,
            entity.IsActive,
            entity.IsDeleted,
            entity.LastUpdatedUtc);
}
