using Microsoft.Extensions.Logging;
using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Products.Commands;

public sealed class DeleteProductCommand
{
    private readonly IProductRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly ILogger<DeleteProductCommand> _logger;

    public DeleteProductCommand(IProductRepository repo, IAuditLogWriter auditLogWriter, ICurrentUserAccessor currentUserAccessor, ILogger<DeleteProductCommand> logger)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
        _currentUserAccessor = currentUserAccessor;
        _logger = logger;
    }

    public async Task<AppResult<Unit>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct);
        if (entity is null)
            return new AppResult<Unit>.NotFound($"Product {id} not found.");

        var currentUser = _currentUserAccessor.GetRequiredCurrentUser();
        var before = Snapshot(entity);

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.DeletedAtUtc = DateTime.UtcNow;
        entity.DeletedByUserId = currentUser.UserId;
        entity.DeletedByUserName = currentUser.UserName;
        entity.LastUpdatedUtc = DateTime.UtcNow;
        await _repo.SaveChangesAsync(ct);

        await _auditLogWriter.WriteAsync(
            "Product",
            id.ToString(),
            "Deleted",
            $"Soft deleted product '{entity.Sku} - {entity.Name}'.",
            beforeState: before,
            afterState: Snapshot(entity),
            changedFields: new object[]
            {
                new { field = "IsDeleted", oldValue = false, newValue = true },
                new { field = "IsActive", oldValue = true, newValue = false },
                new { field = "DeletedAtUtc", oldValue = (DateTime?)null, newValue = entity.DeletedAtUtc },
                new { field = "DeletedByUserName", oldValue = (string?)null, newValue = entity.DeletedByUserName }
            },
            ct: ct);
        _logger.LogInformation("Deleted product {ProductId} ({Sku}).", entity.Id, entity.Sku);
        return new AppResult<Unit>.Ok(Unit.Value);
    }

    private static object Snapshot(Product entity) => new
    {
        entity.Id,
        entity.Sku,
        entity.Name,
        entity.Description,
        entity.CategoryId,
        entity.OnHandQty,
        entity.AverageCost,
        entity.ReorderLevel,
        entity.IsActive,
        entity.IsDeleted,
        entity.DeletedAtUtc,
        entity.DeletedByUserName,
        entity.LastUpdatedUtc
    };
}
