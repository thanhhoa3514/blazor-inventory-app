using MyApp.Shared.Contracts;

namespace MyApp.Server.Persistence.Repositories;

public interface IReorderReadRepository
{
    Task<IReadOnlyList<ReorderRecommendationDto>> GetRecommendationsAsync(
        string? search = null,
        int? categoryId = null,
        string? priority = null,
        CancellationToken ct = default);
}
