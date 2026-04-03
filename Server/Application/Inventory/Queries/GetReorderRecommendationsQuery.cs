using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Application.Inventory.Queries;

public sealed class GetReorderRecommendationsQuery
{
    private readonly IReorderReadRepository _repo;

    public GetReorderRecommendationsQuery(IReorderReadRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<ReorderRecommendationDto>> ExecuteAsync(
        string? search,
        int? categoryId,
        string? priority,
        CancellationToken ct = default)
        => _repo.GetRecommendationsAsync(search, categoryId, priority, ct);
}
