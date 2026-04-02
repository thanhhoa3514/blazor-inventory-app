using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Customers.Queries;

public sealed class GetAllCustomersQuery
{
    private readonly ICustomerRepository _repo;

    public GetAllCustomersQuery(ICustomerRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<CustomerDto>> ExecuteAsync(CancellationToken ct = default)
        => _repo.GetAllAsync(ct);
}
