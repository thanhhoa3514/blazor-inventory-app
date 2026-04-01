using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Products.Queries;

public sealed class GetAllProductsQuery
{
    private readonly IProductRepository _repo;

    public GetAllProductsQuery(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<ProductDto>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.GetAllAsync(ct);
}
