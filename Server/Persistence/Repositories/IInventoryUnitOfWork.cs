using MyApp.Shared.Domain;

namespace MyApp.Server.Persistence.Repositories;

/// <summary>
/// Owns the EF transaction scope and tracks new inventory documents/ledger entries.
/// All Add* methods stage entities for save; none of them call SaveChanges.
/// </summary>
public interface IInventoryUnitOfWork : IAsyncDisposable
{
    void AddReceipt(StockReceipt receipt);
    void AddIssue(StockIssue issue);
    void AddAdjustment(StockAdjustment adjustment);
    void AddLedgerEntry(InventoryLedgerEntry entry);

    Task BeginTransactionAsync(CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}
