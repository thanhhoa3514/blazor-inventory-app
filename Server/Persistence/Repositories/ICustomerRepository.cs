using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Persistence.Repositories;

public interface ICustomerRepository
{
    Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken ct = default);
    Task<CustomerDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, int? excludeId, CancellationToken ct = default);
    Task<Customer?> FindAsync(int id, CancellationToken ct = default);
    Task<Customer?> FindActiveAsync(int id, CancellationToken ct = default);
    Task AddAsync(Customer entity, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
