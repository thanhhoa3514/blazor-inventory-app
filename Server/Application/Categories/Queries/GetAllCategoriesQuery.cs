using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Categories.Queries;

public sealed class GetAllCategoriesQuery
{
    private readonly ICategoryRepository _repo;

    public GetAllCategoriesQuery(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<CategoryDto>> ExecuteAsync(CancellationToken ct = default)
        => await _repo.GetAllAsync(ct);
}
