using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;

namespace MyApp.Server.Application.Customers.Commands;

public sealed class DeactivateCustomerCommand
{
    private readonly ICustomerRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;

    public DeactivateCustomerCommand(ICustomerRepository repo, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
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
            await _auditLogWriter.WriteAsync("Customer", entity.Id.ToString(), "Deactivated", $"Deactivated customer '{entity.Name}'.", ct);
        }

        return new AppResult<Unit>.Ok(Unit.Value);
    }
}
