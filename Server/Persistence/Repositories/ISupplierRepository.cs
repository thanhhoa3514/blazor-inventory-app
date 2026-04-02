using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Persistence.Repositories;

public interface ISupplierRepository
{
    Task<IReadOnlyList<SupplierDto>> GetAllAsync(CancellationToken ct = default);
    Task<SupplierDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, int? excludeId, CancellationToken ct = default);
    Task<Supplier?> FindAsync(int id, CancellationToken ct = default);
    Task<Supplier?> FindActiveAsync(int id, CancellationToken ct = default);
    Task AddAsync(Supplier entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
