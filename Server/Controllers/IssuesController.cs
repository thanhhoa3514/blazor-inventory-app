using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Server.Services;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Controllers;

[ApiController]
[Route("api/issues")]
public class IssuesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<IssuesController> _logger;

    public IssuesController(AppDbContext db, IInventoryService inventoryService, ILogger<IssuesController> logger)
    {
        _db = db;
        _inventoryService = inventoryService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockIssueListDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _db.StockIssues.AsNoTracking()
            .OrderByDescending(x => x.IssuedAtUtc)
            .Select(x => new StockIssueListDto(
                x.Id,
                x.DocumentNo,
                x.IssuedAtUtc,
                x.Customer,
                x.TotalAmount,
                x.Lines.Count))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StockIssueDetailDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _db.StockIssues.AsNoTracking()
            .Include(x => x.Lines)
            .ThenInclude(x => x.Product)
            .Where(x => x.Id == id)
            .Select(x => new StockIssueDetailDto
            {
                Id = x.Id,
                DocumentNo = x.DocumentNo,
                IssuedAtUtc = x.IssuedAtUtc,
                Customer = x.Customer,
                Note = x.Note,
                TotalAmount = x.TotalAmount,
                Lines = x.Lines
                    .OrderBy(l => l.Id)
                    .Select(l => new StockIssueDetailLineDto(
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
    public async Task<ActionResult<StockIssueDetailDto>> Create(CreateStockIssueRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var created = await _inventoryService.CreateIssueAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InventoryValidationException ex)
        {
            _logger.LogWarning("Issue validation failed: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating issue.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected server error.");
        }
    }
}
