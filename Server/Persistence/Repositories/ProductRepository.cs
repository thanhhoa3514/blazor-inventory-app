using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Persistence.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct = default)
        => await _db.Products.AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Include(x => x.Category)
            .Include(x => x.PreferredSupplier)
            .OrderBy(x => x.Name)
            .Select(x => new ProductDto(
                x.Id,
                x.Sku,
                x.Name,
                x.Description,
                x.CategoryId,
                x.Category!.Name,
                x.PreferredSupplierId,
                x.PreferredSupplier != null ? x.PreferredSupplier.Name : null,
                x.OnHandQty,
                x.AverageCost,
                x.ReorderLevel,
                x.TargetStockLevel,
                x.IsActive,
                x.IsDeleted,
                x.LastUpdatedUtc))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<ProductDto>> GetDeletedAsync(CancellationToken ct = default)
        => await _db.Products.AsNoTracking()
            .Where(x => x.IsDeleted)
            .Include(x => x.Category)
            .Include(x => x.PreferredSupplier)
            .OrderBy(x => x.Name)
            .Select(x => new ProductDto(
                x.Id,
                x.Sku,
                x.Name,
                x.Description,
                x.CategoryId,
                x.Category != null ? x.Category.Name : "Uncategorized",
                x.PreferredSupplierId,
                x.PreferredSupplier != null ? x.PreferredSupplier.Name : null,
                x.OnHandQty,
                x.AverageCost,
                x.ReorderLevel,
                x.TargetStockLevel,
                x.IsActive,
                x.IsDeleted,
                x.LastUpdatedUtc))
            .ToListAsync(ct);

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Products.AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Include(x => x.Category)
            .Include(x => x.PreferredSupplier)
            .Where(x => x.Id == id)
            .Select(x => new ProductDto(
                x.Id,
                x.Sku,
                x.Name,
                x.Description,
                x.CategoryId,
                x.Category!.Name,
                x.PreferredSupplierId,
                x.PreferredSupplier != null ? x.PreferredSupplier.Name : null,
                x.OnHandQty,
                x.AverageCost,
                x.ReorderLevel,
                x.TargetStockLevel,
                x.IsActive,
                x.IsDeleted,
                x.LastUpdatedUtc))
            .FirstOrDefaultAsync(ct);

    public async Task<bool> CategoryExistsAsync(int categoryId, CancellationToken ct = default)
        => await _db.Categories.AnyAsync(x => x.Id == categoryId && !x.IsDeleted, ct);

    public async Task<bool> PreferredSupplierExistsAsync(int supplierId, CancellationToken ct = default)
        => await _db.Suppliers.AnyAsync(x => x.Id == supplierId && x.IsActive && !x.IsDeleted, ct);

    public async Task<bool> ExistsBySkuAsync(string sku, int? excludeId, CancellationToken ct = default)
        => excludeId.HasValue
            ? await _db.Products.AnyAsync(x => x.Id != excludeId.Value && !x.IsDeleted && x.Sku == sku, ct)
            : await _db.Products.AnyAsync(x => !x.IsDeleted && x.Sku == sku, ct);

    public async Task<bool> HasTransactionHistoryAsync(int id, CancellationToken ct = default)
        => await _db.StockReceiptLines.AnyAsync(x => x.ProductId == id, ct) ||
           await _db.StockIssueLines.AnyAsync(x => x.ProductId == id, ct) ||
           await _db.StockAdjustmentLines.AnyAsync(x => x.ProductId == id, ct);

    public async Task<Product?> FindAsync(int id, CancellationToken ct = default)
        => await _db.Products.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

    public async Task<Product?> FindIncludingDeletedAsync(int id, CancellationToken ct = default)
        => await _db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Product?> FindActiveAsync(int id, CancellationToken ct = default)
        => await _db.Products.FirstOrDefaultAsync(x => x.Id == id && x.IsActive && !x.IsDeleted, ct);

    public async Task AddAsync(Product entity, CancellationToken ct = default)
    {
        _db.Products.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);

    public async Task DeleteAsync(Product entity, CancellationToken ct = default)
    {
        _db.Products.Update(entity);
        await _db.SaveChangesAsync(ct);
    }
}
