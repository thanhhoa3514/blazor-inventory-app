using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Suppliers.Queries;

public sealed class GetDeletedSuppliersQuery
{
    private readonly ISupplierRepository _repo;

    public GetDeletedSuppliersQuery(ISupplierRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<SupplierDto>> ExecuteAsync(CancellationToken ct = default)
        => _repo.GetDeletedAsync(ct);
}
