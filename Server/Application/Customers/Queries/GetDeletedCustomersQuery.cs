using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Customers.Queries;

public sealed class GetDeletedCustomersQuery
{
    private readonly ICustomerRepository _repo;

    public GetDeletedCustomersQuery(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<CustomerDto>> ExecuteAsync(CancellationToken ct = default)
        => _repo.GetDeletedAsync(ct);
}
