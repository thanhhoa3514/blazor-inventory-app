using MyApp.Shared.Contracts;

namespace MyApp.Server.Persistence.Repositories;

public interface IStockAdjustmentRepository
{
    Task<IReadOnlyList<StockAdjustmentListDto>> GetAllAsync(CancellationToken ct = default);
    Task<StockAdjustmentDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
}
