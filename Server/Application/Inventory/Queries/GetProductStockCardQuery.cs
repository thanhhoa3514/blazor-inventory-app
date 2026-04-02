using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Inventory.Queries;

public sealed class GetProductStockCardQuery
{
    private readonly IInventoryReadRepository _repo;

    public GetProductStockCardQuery(IInventoryReadRepository repo)
    {
        _repo = repo;
    }

    public async Task<AppResult<ProductStockCardDto>> ExecuteAsync(
        int productId,
        DateTime? fromUtc,
        DateTime? toUtc,
        string? movementType,
        CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(movementType))
        {
            var normalized = movementType.Trim().ToUpperInvariant();
            if (!InventoryMovementTypes.All.Contains(normalized))
            {
                return new AppResult<ProductStockCardDto>.ValidationError("Movement type must be RECEIPT, ISSUE, or ADJUSTMENT.");
            }

            movementType = normalized;
        }

        var item = await _repo.GetProductStockCardAsync(productId, fromUtc, toUtc, movementType, ct);
        return item is null
            ? new AppResult<ProductStockCardDto>.NotFound($"Product with ID {productId} was not found.")
            : new AppResult<ProductStockCardDto>.Ok(item);
    }
}
