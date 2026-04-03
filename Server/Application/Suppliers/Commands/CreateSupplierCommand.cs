using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Suppliers.Commands;

public sealed class CreateSupplierCommand
{
    private readonly ISupplierRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;

    public CreateSupplierCommand(ISupplierRepository repo, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
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
            IsDeleted = false,
            CreatedAtUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        await _auditLogWriter.WriteAsync(
            "Supplier",
            entity.Id.ToString(),
            "Created",
            $"Created supplier '{entity.Name}'.",
            afterState: Snapshot(entity),
            changedFields: new object[]
            {
                new { field = "Name", oldValue = (string?)null, newValue = entity.Name },
                new { field = "Description", oldValue = (string?)null, newValue = entity.Description },
                new { field = "IsActive", oldValue = (bool?)null, newValue = entity.IsActive }
            },
            ct: ct);
        return new AppResult<SupplierDto>.Ok(entity.ToDto());
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
