using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Categories.Queries;

public sealed class GetCategoryByIdQuery
{
    private readonly ICategoryRepository _repo;

    public GetCategoryByIdQuery(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<AppResult<CategoryDto>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var dto = await _repo.GetByIdAsync(id, ct);
        if (dto is null)
            return new AppResult<CategoryDto>.NotFound($"Category {id} not found.");
        return new AppResult<CategoryDto>.Ok(dto);
    }
}
