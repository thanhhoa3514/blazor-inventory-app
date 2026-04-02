using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Persistence.Repositories;

public sealed class InventoryReadRepository : IInventoryReadRepository
{
    private readonly AppDbContext _db;

    public InventoryReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ProductStockCardDto?> GetProductStockCardAsync(
        int productId,
        DateTime? fromUtc,
        DateTime? toUtc,
        string? movementType,
        CancellationToken ct = default)
    {
        var product = await _db.Products.AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.Id == productId)
            .Select(x => new
            {
                x.Id,
                x.Sku,
                x.Name,
                x.Description,
                CategoryName = x.Category != null ? x.Category.Name : "Uncategorized",
                x.IsActive,
                x.OnHandQty,
                x.AverageCost
            })
            .FirstOrDefaultAsync(ct);

        if (product is null)
            return null;

        var query = _db.InventoryLedgerEntries.AsNoTracking()
            .Where(x => x.ProductId == productId);

        if (fromUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(x => x.OccurredAtUtc <= toUtc.Value);

        if (!string.IsNullOrWhiteSpace(movementType))
        {
            var normalized = movementType.Trim().ToUpperInvariant();
            query = query.Where(x => x.MovementType == normalized);
        }

        var entries = await query
            .OrderByDescending(x => x.OccurredAtUtc)
            .ThenByDescending(x => x.Id)
            .Select(x => new StockCardEntryDto(
                x.Id,
                x.OccurredAtUtc,
                x.MovementType,
                x.ReferenceNo,
                x.QuantityChange,
                x.UnitCost,
                x.ValueChange,
                x.RunningOnHandQty,
                x.RunningAverageCost))
            .ToListAsync(ct);

        return new ProductStockCardDto
        {
            ProductId = product.Id,
            Sku = product.Sku,
            Name = product.Name,
            Description = product.Description,
            CategoryName = product.CategoryName,
            IsActive = product.IsActive,
            CurrentOnHandQty = product.OnHandQty,
            CurrentAverageCost = product.AverageCost,
            Entries = entries
        };
    }
}
