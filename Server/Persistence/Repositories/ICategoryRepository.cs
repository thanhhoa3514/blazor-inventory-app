using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Persistence.Repositories;

public interface ICategoryRepository
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken ct = default);
    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, int? excludeId, CancellationToken ct = default);
    Task<bool> HasProductsAsync(int id, CancellationToken ct = default);
    Task<Category?> FindAsync(int id, CancellationToken ct = default);
    Task AddAsync(Category entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task DeleteAsync(Category entity, CancellationToken ct = default);
}
