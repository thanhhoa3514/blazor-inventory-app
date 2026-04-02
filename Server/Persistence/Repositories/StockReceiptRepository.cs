using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Persistence.Repositories;

public sealed class StockReceiptRepository : IStockReceiptRepository
{
    private readonly AppDbContext _db;

    public StockReceiptRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<StockReceiptListDto>> GetAllAsync(CancellationToken ct = default)
        => await _db.StockReceipts.AsNoTracking()
            .Include(x => x.Supplier)
            .OrderByDescending(x => x.ReceivedAtUtc)
            .Select(x => new StockReceiptListDto(
                x.Id,
                x.DocumentNo,
                x.ReceivedAtUtc,
                x.SupplierId,
                x.Supplier != null ? x.Supplier.Name : null,
                x.TotalAmount,
                x.Lines.Count))
            .ToListAsync(ct);

    public async Task<StockReceiptDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.StockReceipts.AsNoTracking()
            .Include(x => x.Supplier)
            .Include(x => x.Lines)
            .ThenInclude(x => x.Product)
            .Where(x => x.Id == id)
            .Select(x => new StockReceiptDetailDto
            {
                Id = x.Id,
                DocumentNo = x.DocumentNo,
                ReceivedAtUtc = x.ReceivedAtUtc,
                SupplierId = x.SupplierId,
                Supplier = x.Supplier != null ? x.Supplier.Name : null,
                Note = x.Note,
                TotalAmount = x.TotalAmount,
                Lines = x.Lines
                    .OrderBy(l => l.Id)
                    .Select(l => new StockReceiptDetailLineDto(
                        l.ProductId,
                        l.Product!.Sku,
                        l.Product.Name,
                        l.Quantity,
                        l.UnitCost,
                        l.LineTotal))
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);
}
