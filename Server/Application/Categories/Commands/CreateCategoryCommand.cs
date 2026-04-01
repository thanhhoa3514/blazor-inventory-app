using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Categories.Commands;

public sealed class CreateCategoryCommand
{
    private readonly ICategoryRepository _repo;

    public CreateCategoryCommand(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<AppResult<CategoryDto>> ExecuteAsync(CreateCategoryRequest request, CancellationToken ct = default)
    {
        var normalizedName = request.Name.Trim();

        if (await _repo.ExistsByNameAsync(normalizedName, excludeId: null, ct))
            return new AppResult<CategoryDto>.Conflict("Category name must be unique.");

        var entity = new Category
        {
            Name = normalizedName,
            Description = request.Description?.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        return new AppResult<CategoryDto>.Ok(entity.ToDto());
    }
}
