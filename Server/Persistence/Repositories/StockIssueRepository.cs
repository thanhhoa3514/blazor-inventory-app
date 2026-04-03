using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Persistence.Repositories;

public sealed class StockIssueRepository : IStockIssueRepository
{
    private readonly AppDbContext _db;

    public StockIssueRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<StockIssueListDto>> GetAllAsync(CancellationToken ct = default)
        => await _db.StockIssues.AsNoTracking()
            .Include(x => x.Customer)
            .OrderByDescending(x => x.IssuedAtUtc)
            .Select(x => new StockIssueListDto(
                x.Id,
                x.DocumentNo,
                x.IssuedAtUtc,
                x.CustomerId,
                x.Customer != null ? x.Customer.Name : null,
                x.TotalAmount,
                x.Lines.Count))
            .ToListAsync(ct);

    public async Task<StockIssueDetailDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.StockIssues.AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Lines)
            .ThenInclude(x => x.Product)
            .Where(x => x.Id == id)
            .Select(x => new StockIssueDetailDto
            {
                Id = x.Id,
                DocumentNo = x.DocumentNo,
                IssuedAtUtc = x.IssuedAtUtc,
                CreatedByUserName = x.CreatedByUserName,
                CustomerId = x.CustomerId,
                Customer = x.Customer != null ? x.Customer.Name : null,
                Note = x.Note,
                TotalAmount = x.TotalAmount,
                Lines = x.Lines
                    .OrderBy(l => l.Id)
                    .Select(l => new StockIssueDetailLineDto(
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
