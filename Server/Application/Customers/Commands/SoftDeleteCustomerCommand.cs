using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;

namespace MyApp.Server.Application.Customers.Commands;

public sealed class SoftDeleteCustomerCommand
{
    private readonly ICustomerRepository _repo;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly IAuditLogWriter _auditLogWriter;

    public SoftDeleteCustomerCommand(ICustomerRepository repo, ICurrentUserAccessor currentUserAccessor, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _currentUserAccessor = currentUserAccessor;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<AppResult<Unit>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct);
        if (entity is null)
            return new AppResult<Unit>.NotFound($"Customer with ID {id} was not found.");

        var currentUser = _currentUserAccessor.GetRequiredCurrentUser();
        var before = new
        {
            entity.Id,
            entity.Name,
            entity.Description,
            entity.IsActive,
            entity.IsDeleted,
            entity.DeletedAtUtc,
            entity.DeletedByUserName
        };

        entity.IsDeleted = true;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.DeletedByUserId = currentUser.UserId;
        entity.DeletedByUserName = currentUser.UserName;
        entity.LastUpdatedUtc = entity.DeletedAtUtc.Value;

        await _repo.SaveChangesAsync(ct);
        await _auditLogWriter.WriteAsync(
            "Customer",
            id.ToString(),
            "Deleted",
            $"Soft deleted customer '{entity.Name}'.",
            beforeState: before,
            afterState: new
            {
                entity.Id,
                entity.Name,
                entity.Description,
                entity.IsActive,
                entity.IsDeleted,
                entity.DeletedAtUtc,
                entity.DeletedByUserName
            },
            changedFields: new object[]
            {
                new { field = "IsDeleted", oldValue = false, newValue = true },
                new { field = "DeletedByUserName", oldValue = (string?)null, newValue = entity.DeletedByUserName }
            },
            ct: ct);

        return new AppResult<Unit>.Ok(Unit.Value);
    }
}
