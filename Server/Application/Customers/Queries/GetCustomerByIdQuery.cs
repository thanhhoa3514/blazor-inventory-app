using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Customers.Queries;

public sealed class GetCustomerByIdQuery
{
    private readonly ICustomerRepository _repo;

    public GetCustomerByIdQuery(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public async Task<AppResult<CustomerDto>> ExecuteAsync(int id, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(id, ct);
        return item is null
            ? new AppResult<CustomerDto>.NotFound($"Customer with ID {id} was not found.")
            : new AppResult<CustomerDto>.Ok(item);
    }
}
