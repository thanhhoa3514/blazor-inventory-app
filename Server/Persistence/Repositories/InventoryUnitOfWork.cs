using Microsoft.EntityFrameworkCore.Storage;
using MyApp.Server.Data;
using MyApp.Shared.Domain;

namespace MyApp.Server.Persistence.Repositories;

public sealed class InventoryUnitOfWork : IInventoryUnitOfWork
{
    private readonly AppDbContext _db;
    private IDbContextTransaction? _tx;

    public InventoryUnitOfWork(AppDbContext db)
    {
        _db = db;
    }

    public void AddReceipt(StockReceipt receipt) => _db.StockReceipts.Add(receipt);
    public void AddIssue(StockIssue issue) => _db.StockIssues.Add(issue);
    public void AddAdjustment(StockAdjustment adjustment) => _db.StockAdjustments.Add(adjustment);
    public void AddLedgerEntry(InventoryLedgerEntry entry) => _db.InventoryLedgerEntries.Add(entry);

    public async Task BeginTransactionAsync(CancellationToken ct = default)
        => _tx = await _db.Database.BeginTransactionAsync(ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_tx is not null)
        {
            await _tx.CommitAsync(ct);
            await _tx.DisposeAsync();
            _tx = null;
        }
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_tx is not null)
        {
            await _tx.RollbackAsync(ct);
            await _tx.DisposeAsync();
            _tx = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_tx is not null)
        {
            await _tx.DisposeAsync();
            _tx = null;
        }
    }
}
