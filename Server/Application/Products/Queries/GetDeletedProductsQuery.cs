using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Products.Queries;

public sealed class GetDeletedProductsQuery
{
    private readonly IProductRepository _repo;

    public GetDeletedProductsQuery(IProductRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<ProductDto>> ExecuteAsync(CancellationToken ct = default)
        => _repo.GetDeletedAsync(ct);
}
