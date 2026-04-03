using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Customers.Commands;

public sealed class UpdateCustomerCommand
{
    private readonly ICustomerRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;

    public UpdateCustomerCommand(ICustomerRepository repo, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<AppResult<CustomerDto>> ExecuteAsync(int id, UpdateCustomerRequest request, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct);
        if (entity is null)
            return new AppResult<CustomerDto>.NotFound($"Customer with ID {id} was not found.");

        var before = Snapshot(entity);
        var oldName = entity.Name;
        var oldDescription = entity.Description;
        var oldIsActive = entity.IsActive;

        var normalizedName = request.Name.Trim();
        if (await _repo.ExistsByNameAsync(normalizedName, id, ct))
            return new AppResult<CustomerDto>.Conflict("Customer name must be unique.");

        entity.Name = normalizedName;
        entity.Description = request.Description?.Trim();
        entity.IsActive = request.IsActive;
        entity.LastUpdatedUtc = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);
        await _auditLogWriter.WriteAsync(
            "Customer",
            entity.Id.ToString(),
            "Updated",
            $"Updated customer '{entity.Name}'.",
            beforeState: before,
            afterState: Snapshot(entity),
            changedFields: BuildChangedFields(oldName, entity.Name, oldDescription, entity.Description, oldIsActive, entity.IsActive),
            ct: ct);
        return new AppResult<CustomerDto>.Ok(entity.ToDto());
    }

    private static object Snapshot(Customer entity) => new
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

    private static object[] BuildChangedFields(string oldName, string newName, string? oldDescription, string? newDescription, bool oldIsActive, bool newIsActive)
    {
        var changed = new List<object>();
        if (!string.Equals(oldName, newName, StringComparison.Ordinal))
            changed.Add(new { field = "Name", oldValue = oldName, newValue = newName });
        if (!string.Equals(oldDescription, newDescription, StringComparison.Ordinal))
            changed.Add(new { field = "Description", oldValue = oldDescription, newValue = newDescription });
        if (oldIsActive != newIsActive)
            changed.Add(new { field = "IsActive", oldValue = oldIsActive, newValue = newIsActive });
        return changed.ToArray();
    }
}
