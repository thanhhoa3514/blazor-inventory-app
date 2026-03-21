using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(AppDbContext db, ILogger<ProductsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(CancellationToken cancellationToken)
    {
        var products = await _db.Products.AsNoTracking()
            .Include(x => x.Category)
            .OrderBy(x => x.Name)
            .Select(x => new ProductDto(
                x.Id,
                x.Sku,
                x.Name,
                x.Description,
                x.CategoryId,
                x.Category!.Name,
                x.OnHandQty,
                x.AverageCost,
                x.ReorderLevel,
                x.IsActive,
                x.LastUpdatedUtc))
            .ToListAsync(cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _db.Products.AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.Id == id)
            .Select(x => new ProductDto(
                x.Id,
                x.Sku,
                x.Name,
                x.Description,
                x.CategoryId,
                x.Category!.Name,
                x.OnHandQty,
                x.AverageCost,
                x.ReorderLevel,
                x.IsActive,
                x.LastUpdatedUtc))
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var categoryExists = await _db.Categories.AnyAsync(x => x.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            return BadRequest("Category does not exist.");
        }

        var normalizedSku = request.Sku.Trim().ToUpperInvariant();
        if (await _db.Products.AnyAsync(x => x.Sku == normalizedSku, cancellationToken))
        {
            return BadRequest("Product SKU must be unique.");
        }

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

        _db.Products.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToDto(entity, await LoadCategoryNameAsync(entity.CategoryId, cancellationToken), entity.LastUpdatedUtc));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductDto>> Update(int id, UpdateProductRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var entity = await _db.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var categoryExists = await _db.Categories.AnyAsync(x => x.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            return BadRequest("Category does not exist.");
        }

        var normalizedSku = request.Sku.Trim().ToUpperInvariant();
        var skuTaken = await _db.Products.AnyAsync(x => x.Id != id && x.Sku == normalizedSku, cancellationToken);
        if (skuTaken)
        {
            return BadRequest("Product SKU must be unique.");
        }

        entity.Sku = normalizedSku;
        entity.Name = request.Name.Trim();
        entity.Description = request.Description?.Trim();
        entity.CategoryId = request.CategoryId;
        entity.ReorderLevel = request.ReorderLevel;
        entity.IsActive = request.IsActive;
        entity.LastUpdatedUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return Ok(ToDto(entity, await LoadCategoryNameAsync(entity.CategoryId, cancellationToken), entity.LastUpdatedUtc));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await _db.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var hasTransactions = await _db.StockReceiptLines.AnyAsync(x => x.ProductId == id, cancellationToken) ||
                              await _db.StockIssueLines.AnyAsync(x => x.ProductId == id, cancellationToken);
        if (hasTransactions)
        {
            return BadRequest("Cannot delete product with transaction history.");
        }

        _db.Products.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Deleted product {ProductId} ({Sku}).", entity.Id, entity.Sku);
        return NoContent();
    }

    private async Task<string> LoadCategoryNameAsync(int categoryId, CancellationToken cancellationToken)
    {
        return await _db.Categories.AsNoTracking()
            .Where(x => x.Id == categoryId)
            .Select(x => x.Name)
            .FirstAsync(cancellationToken);
    }

    private static ProductDto ToDto(Product entity, string categoryName, DateTime lastUpdatedUtc)
        => new(
            entity.Id,
            entity.Sku,
            entity.Name,
            entity.Description,
            entity.CategoryId,
            categoryName,
            entity.OnHandQty,
            entity.AverageCost,
            entity.ReorderLevel,
            entity.IsActive,
            lastUpdatedUtc);
}
