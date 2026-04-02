using MyApp.Shared.Contracts;

namespace MyApp.Server.Persistence.Repositories;

public interface IInventoryReadRepository
{
    Task<ProductStockCardDto?> GetProductStockCardAsync(
        int productId,
        DateTime? fromUtc,
        DateTime? toUtc,
        string? movementType,
        CancellationToken ct = default);
}
