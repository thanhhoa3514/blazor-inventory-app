using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Categories.Commands;

public sealed class UpdateCategoryCommand
{
    private readonly ICategoryRepository _repo;

    public UpdateCategoryCommand(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<AppResult<CategoryDto>> ExecuteAsync(int id, UpdateCategoryRequest request, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct);
        if (entity is null)
            return new AppResult<CategoryDto>.NotFound($"Category {id} not found.");

        var normalizedName = request.Name.Trim();
        if (await _repo.ExistsByNameAsync(normalizedName, excludeId: id, ct))
            return new AppResult<CategoryDto>.Conflict("Category name must be unique.");

        entity.Name = normalizedName;
        entity.Description = request.Description?.Trim();

        await _repo.SaveChangesAsync(ct);
        return new AppResult<CategoryDto>.Ok(entity.ToDto());
    }
}
