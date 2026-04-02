using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Suppliers.Commands;

public sealed class CreateSupplierCommand
{
    private readonly ISupplierRepository _repo;

    public CreateSupplierCommand(ISupplierRepository repo)
    {
        _repo = repo;
    }

    public async Task<AppResult<SupplierDto>> ExecuteAsync(CreateSupplierRequest request, CancellationToken ct = default)
    {
        var normalizedName = request.Name.Trim();
        if (await _repo.ExistsByNameAsync(normalizedName, excludeId: null, ct))
            return new AppResult<SupplierDto>.Conflict("Supplier name must be unique.");

        var entity = new Supplier
        {
            Name = normalizedName,
            Description = request.Description?.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        return new AppResult<SupplierDto>.Ok(entity.ToDto());
    }
}
