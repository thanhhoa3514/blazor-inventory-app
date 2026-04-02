using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Suppliers.Queries;

public sealed class GetSupplierByIdQuery
{
    private readonly ISupplierRepository _repo;

    public GetSupplierByIdQuery(ISupplierRepository repo)
    {
        _repo = repo;
    }

    public async Task<AppResult<SupplierDto>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(id, ct);
        return item is null
            ? new AppResult<SupplierDto>.NotFound($"Supplier with ID {id} was not found.")
            : new AppResult<SupplierDto>.Ok(item);
    }
}
