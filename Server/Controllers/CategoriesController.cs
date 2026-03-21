using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var items = await _db.Categories.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CategoryDto(x.Id, x.Name, x.Description, x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await _db.Categories.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new CategoryDto(x.Id, x.Name, x.Description, x.CreatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);

        if (item is null)
        {
            return NotFound();
        }

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var exists = await _db.Categories.AnyAsync(x => x.Name == request.Name.Trim(), cancellationToken);
        if (exists)
        {
            return BadRequest("Category name must be unique.");
        }

        var entity = new Category
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        _db.Categories.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToDto(entity));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoryDto>> Update(int id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var entity = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var normalizedName = request.Name.Trim();
        var nameTaken = await _db.Categories.AnyAsync(x => x.Id != id && x.Name == normalizedName, cancellationToken);
        if (nameTaken)
        {
            return BadRequest("Category name must be unique.");
        }

        entity.Name = normalizedName;
        entity.Description = request.Description?.Trim();
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(ToDto(entity));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var entity = await _db.Categories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return NotFound();
        }

        var hasProducts = await _db.Products.AnyAsync(x => x.CategoryId == id, cancellationToken);
        if (hasProducts)
        {
            return BadRequest("Cannot delete category while products exist.");
        }

        _db.Categories.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static CategoryDto ToDto(Category entity)
        => new(entity.Id, entity.Name, entity.Description, entity.CreatedAtUtc);
}
