using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Auth;
using MyApp.Server.Data;
using MyApp.Server.Services;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Controllers;

[ApiController]
[Route("api/receipts")]
[Authorize(Policy = AppPolicies.ReadAccess)]
public class ReceiptsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<ReceiptsController> _logger;

    public ReceiptsController(AppDbContext db, IInventoryService inventoryService, ILogger<ReceiptsController> logger)
    {
        _db = db;
        _inventoryService = inventoryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockReceiptListDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _db.StockReceipts.AsNoTracking()
            .OrderByDescending(x => x.ReceivedAtUtc)
            .Select(x => new StockReceiptListDto(
                x.Id,
                x.DocumentNo,
                x.ReceivedAtUtc,
                x.Supplier,
                x.TotalAmount,
                x.Lines.Count))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StockReceiptDetailDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _db.StockReceipts.AsNoTracking()
            .Include(x => x.Lines)
            .ThenInclude(x => x.Product)
            .Where(x => x.Id == id)
            .Select(x => new StockReceiptDetailDto
            {
                Id = x.Id,
                DocumentNo = x.DocumentNo,
                ReceivedAtUtc = x.ReceivedAtUtc,
                Supplier = x.Supplier,
                Note = x.Note,
                TotalAmount = x.TotalAmount,
                Lines = x.Lines
                    .OrderBy(l => l.Id)
                    .Select(l => new StockReceiptDetailLineDto(
                        l.ProductId,
                        l.Product!.Sku,
                        l.Product.Name,
                        l.Quantity,
                        l.UnitCost,
                        l.LineTotal))
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    [Authorize(Policy = AppPolicies.WarehouseOperations)]
    public async Task<ActionResult<StockReceiptDetailDto>> Create(CreateStockReceiptRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var created = await _inventoryService.CreateReceiptAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InventoryValidationException ex)
        {
            _logger.LogWarning("Receipt validation failed: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating receipt.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected server error.");
        }
    }
}
