using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Persistence.Repositories;

public sealed class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _db;

    public SupplierRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<SupplierDto>> GetAllAsync(CancellationToken ct = default)
        => await _db.Suppliers.AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new SupplierDto(
                x.Id,
                x.Name,
                x.Description,
                x.IsActive,
                x.IsDeleted,
                x.CreatedAtUtc,
                x.LastUpdatedUtc))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<SupplierDto>> GetDeletedAsync(CancellationToken ct = default)
        => await _db.Suppliers.AsNoTracking()
            .Where(x => x.IsDeleted)
            .OrderBy(x => x.Name)
            .Select(x => new SupplierDto(
                x.Id,
                x.Name,
                x.Description,
                x.IsActive,
                x.IsDeleted,
                x.CreatedAtUtc,
                x.LastUpdatedUtc))
            .ToListAsync(ct);

    public async Task<SupplierDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Suppliers.AsNoTracking()
            .Where(x => x.Id == id && !x.IsDeleted)
            .Select(x => new SupplierDto(
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
            ? await _db.Suppliers.AnyAsync(x => x.Id != excludeId.Value && !x.IsDeleted && x.Name == name, ct)
            : await _db.Suppliers.AnyAsync(x => !x.IsDeleted && x.Name == name, ct);

    public async Task<Supplier?> FindAsync(int id, CancellationToken ct = default)
        => await _db.Suppliers.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

    public async Task<Supplier?> FindIncludingDeletedAsync(int id, CancellationToken ct = default)
        => await _db.Suppliers.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Supplier?> FindActiveAsync(int id, CancellationToken ct = default)
        => await _db.Suppliers.FirstOrDefaultAsync(x => x.Id == id && x.IsActive && !x.IsDeleted, ct);

    public async Task AddAsync(Supplier entity, CancellationToken ct = default)
    {
        _db.Suppliers.Add(entity);
        await _db.SaveChangesAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}
