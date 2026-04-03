using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Suppliers.Commands;

public sealed class UpdateSupplierCommand
{
    private readonly ISupplierRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;

    public UpdateSupplierCommand(ISupplierRepository repo, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<AppResult<SupplierDto>> ExecuteAsync(int id, UpdateSupplierRequest request, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct);
        if (entity is null)
            return new AppResult<SupplierDto>.NotFound($"Supplier with ID {id} was not found.");

        var normalizedName = request.Name.Trim();
        if (await _repo.ExistsByNameAsync(normalizedName, id, ct))
            return new AppResult<SupplierDto>.Conflict("Supplier name must be unique.");

        entity.Name = normalizedName;
        entity.Description = request.Description?.Trim();
        entity.IsActive = request.IsActive;
        entity.LastUpdatedUtc = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);
        await _auditLogWriter.WriteAsync("Supplier", entity.Id.ToString(), "Updated", $"Updated supplier '{entity.Name}'.", ct);
        return new AppResult<SupplierDto>.Ok(entity.ToDto());
    }
}
