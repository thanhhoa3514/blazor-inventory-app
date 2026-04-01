using MyApp.Shared.Contracts;

namespace MyApp.Server.Persistence.Repositories;

public interface IStockReceiptRepository
{
    Task<IReadOnlyList<StockReceiptListDto>> GetAllAsync(CancellationToken ct = default);
    Task<StockReceiptDetailDto?> GetByIdAsync(int id, CancellationToken ct = default);
}
