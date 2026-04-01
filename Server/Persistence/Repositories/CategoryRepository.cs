using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default)
        => await _db.Categories.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CategoryDto(x.Id, x.Name, x.Description, x.CreatedAtUtc))
            .ToListAsync(ct);

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Categories.AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new CategoryDto(x.Id, x.Name, x.Description, x.CreatedAtUtc))
            .FirstOrDefaultAsync(ct);

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId, CancellationToken ct = default)
        => excludeId.HasValue
            ? await _db.Categories.AnyAsync(x => x.Id != excludeId.Value && x.Name == name, ct)
            : await _db.Categories.AnyAsync(x => x.Name == name, ct);

    public async Task<bool> HasProductsAsync(int id, CancellationToken ct = default)
        => await _db.Products.AnyAsync(x => x.CategoryId == id, ct);

    public async Task<Category?> FindAsync(int id, CancellationToken ct = default)
        => await _db.Categories.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task AddAsync(Category entity, CancellationToken ct = default)
    {
        _db.Categories.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);

    public async Task DeleteAsync(Category entity, CancellationToken ct = default)
    {
        _db.Categories.Remove(entity);
        await _db.SaveChangesAsync(ct);
    }
}
