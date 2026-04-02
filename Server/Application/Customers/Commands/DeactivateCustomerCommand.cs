using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;

namespace MyApp.Server.Application.Customers.Commands;

public sealed class DeactivateCustomerCommand
{
    private readonly ICustomerRepository _repo;

    public DeactivateCustomerCommand(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<AppResult<Unit>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct);
        if (entity is null)
            return new AppResult<Unit>.NotFound($"Customer with ID {id} was not found.");

        if (entity.IsActive)
        {
            entity.IsActive = false;
            entity.LastUpdatedUtc = DateTime.UtcNow;
            await _repo.SaveChangesAsync(ct);
        }

        return new AppResult<Unit>.Ok(Unit.Value);
    }
}
