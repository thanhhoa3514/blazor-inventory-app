using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Persistence.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ProductDto>> GetDeletedAsync(CancellationToken ct = default);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> CategoryExistsAsync(int categoryId, CancellationToken ct = default);
    Task<bool> ExistsBySkuAsync(string sku, int? excludeId, CancellationToken ct = default);
    Task<bool> HasTransactionHistoryAsync(int id, CancellationToken ct = default);
    Task<Product?> FindAsync(int id, CancellationToken ct = default);
    Task<Product?> FindIncludingDeletedAsync(int id, CancellationToken ct = default);
    Task<Product?> FindActiveAsync(int id, CancellationToken ct = default);
    Task AddAsync(Product entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task DeleteAsync(Product entity, CancellationToken ct = default);
}
