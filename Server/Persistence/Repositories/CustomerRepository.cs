using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Persistence.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;

    public CustomerRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync(CancellationToken ct = default)
        => await _db.Customers.AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new CustomerDto(
                x.Id,
                x.Name,
                x.Description,
                x.IsActive,
                x.IsDeleted,
                x.CreatedAtUtc,
                x.LastUpdatedUtc))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<CustomerDto>> GetDeletedAsync(CancellationToken ct = default)
        => await _db.Customers.AsNoTracking()
            .Where(x => x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new CustomerDto(
                x.Id,
                x.Name,
                x.Description,
                x.IsActive,
                x.IsDeleted,
                x.CreatedAtUtc,
                x.LastUpdatedUtc))
            .ToListAsync(ct);

    public async Task<CustomerDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Customers.AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new CustomerDto(
                x.Id,
                x.Name,
                x.Description,
                x.IsActive,
                x.IsDeleted,
                x.CreatedAtUtc,
                x.LastUpdatedUtc))
            .FirstOrDefaultAsync(ct);

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId, CancellationToken ct = default)
        => excludeId.HasValue
            ? await _db.Customers.AnyAsync(x => x.Id != excludeId.Value && !x.IsDeleted && x.Name == name, ct)
            : await _db.Customers.AnyAsync(x => !x.IsDeleted && x.Name == name, ct);

    public async Task<Customer?> FindAsync(int id, CancellationToken ct = default)
        => await _db.Customers.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

    public async Task<Customer?> FindIncludingDeletedAsync(int id, CancellationToken ct = default)
        => await _db.Customers.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Customer?> FindActiveAsync(int id, CancellationToken ct = default)
        => await _db.Customers.FirstOrDefaultAsync(x => x.Id == id && x.IsActive && !x.IsDeleted, ct);

    public async Task AddAsync(Customer entity, CancellationToken ct = default)
    {
        _db.Customers.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
