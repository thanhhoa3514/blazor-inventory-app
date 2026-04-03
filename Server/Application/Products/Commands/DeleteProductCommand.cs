using Microsoft.Extensions.Logging;
using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;

namespace MyApp.Server.Application.Products.Commands;

public sealed class DeleteProductCommand
{
    private readonly IProductRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;
    private readonly ILogger<DeleteProductCommand> _logger;

    public DeleteProductCommand(IProductRepository repo, IAuditLogWriter auditLogWriter, ILogger<DeleteProductCommand> logger)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
        _logger = logger;
    }

    public async Task<AppResult<Unit>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.FindAsync(id, ct);
        if (entity is null)
            return new AppResult<Unit>.NotFound($"Product {id} not found.");

        if (await _repo.HasTransactionHistoryAsync(id, ct))
            return new AppResult<Unit>.Conflict("Cannot delete product with transaction history.");

        await _repo.DeleteAsync(entity, ct);
        await _auditLogWriter.WriteAsync("Product", id.ToString(), "Deleted", $"Deleted product '{entity.Sku} - {entity.Name}'.", ct);
        _logger.LogInformation("Deleted product {ProductId} ({Sku}).", entity.Id, entity.Sku);
        return new AppResult<Unit>.Ok(Unit.Value);
    }
}
