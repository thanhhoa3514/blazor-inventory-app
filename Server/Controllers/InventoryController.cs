using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Auth;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Controllers;

[ApiController]
[Route("api/inventory")]
[Authorize(Policy = AppPolicies.ReadAccess)]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _db;

    public InventoryController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<InventorySummaryDto>> GetSummary(CancellationToken cancellationToken)
    {
        var products = await _db.Products.AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.IsActive)
            .ToListAsync(cancellationToken);

        var summary = new InventorySummaryDto
        {
            TotalProducts = products.Count,
            TotalOnHandUnits = products.Sum(x => x.OnHandQty),
            TotalInventoryValue = products.Sum(x => Math.Round(x.OnHandQty * x.AverageCost, 2, MidpointRounding.AwayFromZero)),
            LowStockCount = products.Count(x => x.OnHandQty <= x.ReorderLevel),
            LowStockItems = products
                .Where(x => x.OnHandQty <= x.ReorderLevel)
                .OrderBy(x => x.OnHandQty)
                .ThenBy(x => x.Name)
                .Select(x => new LowStockItemDto(
                    x.Id,
                    x.Sku,
                    x.Name,
                    x.OnHandQty,
                    x.ReorderLevel,
                    x.Category?.Name ?? "Uncategorized"))
                .ToList()
        };

        return Ok(summary);
    }
}
