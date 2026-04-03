using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Categories.Queries;

public sealed class GetDeletedCategoriesQuery
{
    private readonly ICategoryRepository _repo;

    public GetDeletedCategoriesQuery(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<CategoryDto>> ExecuteAsync(CancellationToken ct = default)
        => _repo.GetDeletedAsync(ct);
}
