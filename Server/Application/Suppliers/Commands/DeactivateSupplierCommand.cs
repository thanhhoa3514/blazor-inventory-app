using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Suppliers.Commands;

public sealed class DeactivateSupplierCommand
{
    private readonly ISupplierRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;

    public DeactivateSupplierCommand(ISupplierRepository repo, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<AppResult<Unit>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct);
        if (entity is null)
            return new AppResult<Unit>.NotFound($"Supplier with ID {id} was not found.");

        if (entity.IsActive)
        {
            var before = Snapshot(entity);
            entity.IsActive = false;
            entity.LastUpdatedUtc = DateTime.UtcNow;
            await _repo.SaveChangesAsync(ct);
            await _auditLogWriter.WriteAsync(
                "Supplier",
                entity.Id.ToString(),
                "Deactivated",
                $"Deactivated supplier '{entity.Name}'.",
                beforeState: before,
                afterState: Snapshot(entity),
                changedFields: new object[]
                {
                    new { field = "IsActive", oldValue = true, newValue = false }
                },
                ct: ct);
        }

        return new AppResult<Unit>.Ok(Unit.Value);
    }

    private static object Snapshot(Supplier entity) => new
    {
        entity.Id,
        entity.Name,
        entity.Description,
        entity.IsActive,
        entity.IsDeleted,
        entity.DeletedAtUtc,
        entity.DeletedByUserName,
        entity.LastUpdatedUtc
    };
}
