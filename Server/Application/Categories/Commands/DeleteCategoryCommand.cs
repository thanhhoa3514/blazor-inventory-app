using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Categories.Commands;

public sealed class DeleteCategoryCommand
{
    private readonly ICategoryRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;
    private readonly ICurrentUserAccessor _currentUserAccessor;

    public DeleteCategoryCommand(ICategoryRepository repo, IAuditLogWriter auditLogWriter, ICurrentUserAccessor currentUserAccessor)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
        _currentUserAccessor = currentUserAccessor;
    }

    public async Task<AppResult<Unit>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct);
        if (entity is null)
            return new AppResult<Unit>.NotFound($"Category {id} not found.");

        if (await _repo.HasProductsAsync(id, ct))
            return new AppResult<Unit>.Conflict("Cannot delete category while products exist.");

        var currentUser = _currentUserAccessor.GetRequiredCurrentUser();
        var before = Snapshot(entity);

        entity.IsDeleted = true;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.DeletedByUserId = currentUser.UserId;
        entity.DeletedByUserName = currentUser.UserName;
        await _repo.SaveChangesAsync(ct);

        await _auditLogWriter.WriteAsync(
            "Category",
            id.ToString(),
            "Deleted",
            $"Soft deleted category '{entity.Name}'.",
            beforeState: before,
            afterState: Snapshot(entity),
            changedFields: new object[]
            {
                new { field = "IsDeleted", oldValue = false, newValue = true },
                new { field = "DeletedAtUtc", oldValue = (DateTime?)null, newValue = entity.DeletedAtUtc },
                new { field = "DeletedByUserName", oldValue = (string?)null, newValue = entity.DeletedByUserName }
            },
            ct: ct);
        return new AppResult<Unit>.Ok(Unit.Value);
    }

    private static object Snapshot(Category entity) => new
    {
        entity.Id,
        entity.Name,
        entity.Description,
        entity.IsDeleted,
        entity.DeletedAtUtc,
        entity.DeletedByUserName,
        entity.CreatedAtUtc
    };
}
