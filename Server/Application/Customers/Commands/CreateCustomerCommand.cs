using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Customers.Commands;

public sealed class CreateCustomerCommand
{
    private readonly ICustomerRepository _repo;
    private readonly IAuditLogWriter _auditLogWriter;

    public CreateCustomerCommand(ICustomerRepository repo, IAuditLogWriter auditLogWriter)
    {
        _repo = repo;
        _auditLogWriter = auditLogWriter;
    }

    public async Task<AppResult<CustomerDto>> ExecuteAsync(CreateCustomerRequest request, CancellationToken ct = default)
    {
        var normalizedName = request.Name.Trim();
        if (await _repo.ExistsByNameAsync(normalizedName, excludeId: null, ct))
            return new AppResult<CustomerDto>.Conflict("Customer name must be unique.");

        var entity = new Customer
        {
            Name = normalizedName,
            Description = request.Description?.Trim(),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            LastUpdatedUtc = DateTime.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        await _auditLogWriter.WriteAsync("Customer", entity.Id.ToString(), "Created", $"Created customer '{entity.Name}'.", ct);
        return new AppResult<CustomerDto>.Ok(entity.ToDto());
    }
}
