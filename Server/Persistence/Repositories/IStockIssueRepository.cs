using MyApp.Shared.Contracts;

namespace MyApp.Server.Persistence.Repositories;

public interface IStockIssueRepository
{
    Task<IReadOnlyList<StockIssueListDto>> GetAllAsync(CancellationToken ct = default);
    Task<StockIssueDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
}
