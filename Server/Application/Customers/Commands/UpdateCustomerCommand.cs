using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

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

        var normalizedName = request.Name.Trim();
        if (await _repo.ExistsByNameAsync(normalizedName, id, ct))
            return new AppResult<CustomerDto>.Conflict("Customer name must be unique.");

        entity.Name = normalizedName;
        entity.Description = request.Description?.Trim();
        entity.IsActive = request.IsActive;
        entity.LastUpdatedUtc = DateTime.UtcNow;

        await _repo.SaveChangesAsync(ct);
        await _auditLogWriter.WriteAsync("Customer", entity.Id.ToString(), "Updated", $"Updated customer '{entity.Name}'.", ct);
        return new AppResult<CustomerDto>.Ok(entity.ToDto());
    }
}
