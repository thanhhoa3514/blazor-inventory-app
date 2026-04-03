using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Persistence.Repositories;

public sealed class StockAdjustmentRepository : IStockAdjustmentRepository
{
    private readonly AppDbContext _db;

    public StockAdjustmentRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<StockAdjustmentListDto>> GetAllAsync(CancellationToken ct = default)
        => await _db.StockAdjustments.AsNoTracking()
            .OrderByDescending(x => x.AdjustedAtUtc)
            .Select(x => new StockAdjustmentListDto(
                x.Id,
                x.DocumentNo,
                x.AdjustedAtUtc,
                x.Reason,
                x.TotalAmount,
                x.Lines.Count))
            .ToListAsync(ct);

    public async Task<StockAdjustmentDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.StockAdjustments.AsNoTracking()
            .Include(x => x.Lines)
            .ThenInclude(x => x.Product)
            .Where(x => x.Id == id)
            .Select(x => new StockAdjustmentDetailDto
            {
                Id = x.Id,
                DocumentNo = x.DocumentNo,
                AdjustedAtUtc = x.AdjustedAtUtc,
                CreatedByUserName = x.CreatedByUserName,
                Reason = x.Reason,
                Note = x.Note,
                TotalAmount = x.TotalAmount,
                Lines = x.Lines
                    .OrderBy(l => l.Id)
                    .Select(l => new StockAdjustmentDetailLineDto(
                        l.ProductId,
                        l.Product!.Sku,
                        l.Product.Name,
                        l.Direction,
                        l.Quantity,
                        l.UnitCost,
                        l.LineTotal))
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);
}
