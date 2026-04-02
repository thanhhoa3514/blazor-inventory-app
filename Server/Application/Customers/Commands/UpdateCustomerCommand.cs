using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Customers.Commands;

public sealed class UpdateCustomerCommand
{
    private readonly ICustomerRepository _repo;

    public UpdateCustomerCommand(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<AppResult<CustomerDto>> ExecuteAsync(int id, UpdateCustomerRequest request, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct);
        if (entity is null)
            return new AppResult<CustomerDto>.NotFound($"Customer with ID {id} was not found.");

        var normalizedName = request.Name.Trim();
        if (await _repo.ExistsByNameAsync(normalizedName, id, ct))
            return new AppResult<CustomerDto>.Conflict("Customer name must be unique.");

        entity.Name = normalizedName;
        entity.Description = request.Description?.Trim();
        entity.IsActive = request.IsActive;
        entity.LastUpdatedUtc = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);
        return new AppResult<CustomerDto>.Ok(entity.ToDto());
    }
}
