using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Suppliers.Commands;

public sealed class RestoreSupplierCommand
{
    private readonly ISupplierRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;

    public RestoreSupplierCommand(ISupplierRepository repo, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<AppResult<SupplierDto>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.FindIncludingDeletedAsync(id, ct);
        if (entity is null)
            return new AppResult<SupplierDto>.NotFound($"Supplier with ID {id} was not found.");

        if (!entity.IsDeleted)
            return new AppResult<SupplierDto>.Conflict("Supplier is not deleted.");

        var before = Snapshot(entity);

        entity.IsDeleted = false;
        entity.DeletedAtUtc = null;
        entity.DeletedByUserId = null;
        entity.DeletedByUserName = null;
        entity.LastUpdatedUtc = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);
        await _auditLogWriter.WriteAsync(
            "Supplier",
            entity.Id.ToString(),
            "Restored",
            $"Restored supplier '{entity.Name}'.",
            beforeState: before,
            afterState: Snapshot(entity),
            changedFields: new object[]
            {
                new { field = "IsDeleted", oldValue = true, newValue = false },
                new { field = "DeletedByUserName", oldValue = before.DeletedByUserName, newValue = (string?)null }
            },
            ct: ct);

        var dto = await _repo.GetByIdAsync(id, ct);
        return new AppResult<SupplierDto>.Ok(dto!);
    }

    private static dynamic Snapshot(Supplier entity) => new
    {
        entity.Id,
        entity.Name,
        entity.Description,
        entity.IsActive,
        entity.IsDeleted,
        entity.DeletedAtUtc,
        entity.DeletedByUserName
    };
}
