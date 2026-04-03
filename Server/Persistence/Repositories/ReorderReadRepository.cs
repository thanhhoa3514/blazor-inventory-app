using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Persistence.Repositories;

public sealed class ReorderReadRepository : IReorderReadRepository
{
    private readonly AppDbContext _db;

    public ReorderReadRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ReorderRecommendationDto>> GetRecommendationsAsync(
        string? search = null,
        int? categoryId = null,
        string? priority = null,
        CancellationToken ct = default)
    {
        var query = _db.Products.AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.IsActive && !x.IsDeleted && x.OnHandQty <= x.ReorderLevel);

        if (categoryId.HasValue)
            query = query.Where(x => x.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(x =>
                x.Sku.Contains(keyword) ||
                x.Name.Contains(keyword) ||
                (x.Category != null && x.Category.Name.Contains(keyword)));
        }

        var rows = await query
            .Select(x => new
            {
                x.Id,
                x.Sku,
                x.Name,
                CategoryName = x.Category != null ? x.Category.Name : "Uncategorized",
                x.OnHandQty,
                x.ReorderLevel,
                x.TargetStockLevel
            })
            .ToListAsync(ct);

        IEnumerable<ReorderRecommendationDto> result = rows.Select(x =>
        {
            var target = x.TargetStockLevel > 0 ? x.TargetStockLevel : Math.Max(x.ReorderLevel * 2, x.ReorderLevel);
            var suggested = Math.Max(target - x.OnHandQty, 0);

            var stockStatus = x.OnHandQty switch
            {
                <= 0 => ReorderStockStatuses.OutOfStock,
                _ when x.OnHandQty <= Math.Max(1, x.ReorderLevel / 2) => ReorderStockStatuses.Critical,
                _ when x.OnHandQty <= x.ReorderLevel => ReorderStockStatuses.Low,
                _ => ReorderStockStatuses.Healthy
            };

            var recommendationPriority = stockStatus switch
            {
                ReorderStockStatuses.OutOfStock => ReorderRecommendationPriorities.Critical,
                ReorderStockStatuses.Critical => ReorderRecommendationPriorities.Critical,
                ReorderStockStatuses.Low => ReorderRecommendationPriorities.High,
                _ => ReorderRecommendationPriorities.Normal
            };

            return new ReorderRecommendationDto(
                x.Id,
                x.Sku,
                x.Name,
                x.CategoryName,
                x.OnHandQty,
                x.ReorderLevel,
                target,
                suggested,
                recommendationPriority,
                stockStatus);
        });

        if (!string.IsNullOrWhiteSpace(priority))
        {
            var normalized = priority.Trim();
            result = result.Where(x => string.Equals(x.Priority, normalized, StringComparison.OrdinalIgnoreCase));
        }

        return result
            .OrderBy(x => PriorityRank(x.Priority))
            .ThenBy(x => x.OnHandQty)
            .ThenBy(x => x.ProductName)
            .ToList();
    }

    private static int PriorityRank(string priority)
        => priority switch
        {
            ReorderRecommendationPriorities.Critical => 0,
            ReorderRecommendationPriorities.High => 1,
            ReorderRecommendationPriorities.Normal => 2,
            _ => 3
        };
}
