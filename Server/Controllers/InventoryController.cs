using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Server.Application.Common;
using MyApp.Server.Application.Inventory.Queries;
using MyApp.Server.Auth;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Controllers;

[ApiController]
[Route("api/inventory")]
[Authorize(Policy = AppPolicies.ReadAccess)]
public class InventoryController : ControllerBase
{
    private readonly IProductRepository _products;
    private readonly GetProductStockCardQuery _getStockCard;

    public InventoryController(IProductRepository products, GetProductStockCardQuery getStockCard)
    {
        _products = products;
        _getStockCard = getStockCard;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<InventorySummaryDto>> GetSummary(CancellationToken cancellationToken)
    {
        var all = await _products.GetAllAsync(cancellationToken);
        var active = all.Where(x => x.IsActive).ToList();

        var summary = new InventorySummaryDto
        {
            TotalProducts = active.Count,
            TotalOnHandUnits = active.Sum(x => x.OnHandQty),
            TotalInventoryValue = active.Sum(x => Math.Round(x.OnHandQty * x.AverageCost, 2, MidpointRounding.AwayFromZero)),
            LowStockCount = active.Count(x => x.OnHandQty <= x.ReorderLevel),
            LowStockItems = active
                .Where(x => x.OnHandQty <= x.ReorderLevel)
                .OrderBy(x => x.OnHandQty)
                .ThenBy(x => x.Name)
                .Select(x => new LowStockItemDto(
                    x.Id,
                    x.Sku,
                    x.Name,
                    x.OnHandQty,
                    x.ReorderLevel,
                    x.CategoryName))
                .ToList()
        };

        return Ok(summary);
    }

    [HttpGet("products/{productId:int}/stock-card")]
    public async Task<ActionResult<ProductStockCardDto>> GetProductStockCard(
        int productId,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [FromQuery] string? movementType,
        CancellationToken cancellationToken)
    {
        var result = await _getStockCard.ExecuteAsync(productId, fromUtc, toUtc, movementType, cancellationToken);
        return result switch
        {
            AppResult<ProductStockCardDto>.Ok ok => Ok(ok.Value),
            AppResult<ProductStockCardDto>.ValidationError err => BadRequest(err.Message),
            AppResult<ProductStockCardDto>.NotFound => NotFound(),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
