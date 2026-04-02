using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Suppliers.Queries;

public sealed class GetAllSuppliersQuery
{
    private readonly ISupplierRepository _repo;

    public GetAllSuppliersQuery(ISupplierRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<SupplierDto>> ExecuteAsync(CancellationToken ct = default)
        => _repo.GetAllAsync(ct);
}
