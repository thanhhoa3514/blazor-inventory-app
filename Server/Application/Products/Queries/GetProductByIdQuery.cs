using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Products.Queries;

public sealed class GetProductByIdQuery
{
    private readonly IProductRepository _repo;

    public GetProductByIdQuery(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<AppResult<ProductDto>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct);
        if (dto is null)
            return new AppResult<ProductDto>.NotFound($"Product {id} not found.");
        return new AppResult<ProductDto>.Ok(dto);
    }
}
