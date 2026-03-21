using MyApp.Shared.Contracts;

namespace MyApp.Server.Services;

public interface IInventoryService
{
    Task<StockReceiptDetailDto> CreateReceiptAsync(CreateStockReceiptRequest request, CancellationToken cancellationToken = default);
    Task<StockIssueDetailDto> CreateIssueAsync(CreateStockIssueRequest request, CancellationToken cancellationToken = default);
}
