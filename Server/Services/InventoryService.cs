using Microsoft.EntityFrameworkCore;
using MyApp.Server.Data;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Services;

public class InventoryService : IInventoryService
{
    private readonly AppDbContext _db;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(AppDbContext db, ILogger<InventoryService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<StockReceiptDetailDto> CreateReceiptAsync(CreateStockReceiptRequest request, CancellationToken cancellationToken = default)
    {
        ValidateReceiptRequest(request);
        var now = DateTime.UtcNow;
        var documentNo = $"RCPT-{now:yyyyMMddHHmmssfff}";

        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var receipt = new StockReceipt
            {
                DocumentNo = documentNo,
                Supplier = request.Supplier?.Trim(),
                Note = request.Note?.Trim(),
                ReceivedAtUtc = now
            };

            decimal total = 0m;
            foreach (var line in request.Lines)
            {
                var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == line.ProductId && x.IsActive, cancellationToken);
                if (product is null)
                {
                    throw new InventoryValidationException($"Product with ID {line.ProductId} does not exist or is inactive.");
                }

                var lineTotal = Math.Round(line.Quantity * line.UnitCost, 2, MidpointRounding.AwayFromZero);
                var currentValue = Math.Round(product.OnHandQty * product.AverageCost, 2, MidpointRounding.AwayFromZero);
                var newQty = product.OnHandQty + line.Quantity;
                var newValue = currentValue + lineTotal;
                var newAverage = newQty == 0 ? 0 : Math.Round(newValue / newQty, 2, MidpointRounding.AwayFromZero);

                product.OnHandQty = newQty;
                product.AverageCost = newAverage;
                product.LastUpdatedUtc = now;

                receipt.Lines.Add(new StockReceiptLine
                {
                    ProductId = product.Id,
                    Quantity = line.Quantity,
                    UnitCost = line.UnitCost,
                    LineTotal = lineTotal
                });

                _db.InventoryLedgerEntries.Add(new InventoryLedgerEntry
                {
                    ProductId = product.Id,
                    MovementType = "RECEIPT",
                    ReferenceNo = documentNo,
                    OccurredAtUtc = now,
                    QuantityChange = line.Quantity,
                    UnitCost = line.UnitCost,
                    ValueChange = lineTotal,
                    RunningOnHandQty = newQty,
                    RunningAverageCost = newAverage
                });

                total += lineTotal;
            }

            receipt.TotalAmount = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            _db.StockReceipts.Add(receipt);

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            _logger.LogInformation("Created receipt {DocumentNo} with {LineCount} lines and total {TotalAmount}.", receipt.DocumentNo, receipt.Lines.Count, receipt.TotalAmount);

            return await BuildReceiptDetailAsync(receipt.Id, cancellationToken)
                ?? throw new InvalidOperationException("Receipt created but could not be loaded.");
        }
        catch (InventoryValidationException)
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to create stock receipt.");
            throw;
        }
    }

    public async Task<StockIssueDetailDto> CreateIssueAsync(CreateStockIssueRequest request, CancellationToken cancellationToken = default)
    {
        ValidateIssueRequest(request);
        var now = DateTime.UtcNow;
        var documentNo = $"ISS-{now:yyyyMMddHHmmssfff}";

        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var issue = new StockIssue
            {
                DocumentNo = documentNo,
                Customer = request.Customer?.Trim(),
                Note = request.Note?.Trim(),
                IssuedAtUtc = now
            };

            decimal total = 0m;
            foreach (var line in request.Lines)
            {
                var product = await _db.Products.FirstOrDefaultAsync(x => x.Id == line.ProductId && x.IsActive, cancellationToken);
                if (product is null)
                {
                    throw new InventoryValidationException($"Product with ID {line.ProductId} does not exist or is inactive.");
                }

                if (product.OnHandQty < line.Quantity)
                {
                    throw new InventoryValidationException($"Insufficient stock for {product.Sku}. On hand {product.OnHandQty}, requested {line.Quantity}.");
                }

                var unitCost = product.AverageCost;
                var lineTotal = Math.Round(line.Quantity * unitCost, 2, MidpointRounding.AwayFromZero);

                product.OnHandQty -= line.Quantity;
                product.LastUpdatedUtc = now;

                issue.Lines.Add(new StockIssueLine
                {
                    ProductId = product.Id,
                    Quantity = line.Quantity,
                    UnitCost = unitCost,
                    LineTotal = lineTotal
                });

                _db.InventoryLedgerEntries.Add(new InventoryLedgerEntry
                {
                    ProductId = product.Id,
                    MovementType = "ISSUE",
                    ReferenceNo = documentNo,
                    OccurredAtUtc = now,
                    QuantityChange = -line.Quantity,
                    UnitCost = unitCost,
                    ValueChange = -lineTotal,
                    RunningOnHandQty = product.OnHandQty,
                    RunningAverageCost = product.AverageCost
                });

                total += lineTotal;
            }

            issue.TotalAmount = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            _db.StockIssues.Add(issue);

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            _logger.LogInformation("Created issue {DocumentNo} with {LineCount} lines and total {TotalAmount}.", issue.DocumentNo, issue.Lines.Count, issue.TotalAmount);

            return await BuildIssueDetailAsync(issue.Id, cancellationToken)
                ?? throw new InvalidOperationException("Issue created but could not be loaded.");
        }
        catch (InventoryValidationException)
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to create stock issue.");
            throw;
        }
    }

    private async Task<StockReceiptDetailDto?> BuildReceiptDetailAsync(int id, CancellationToken cancellationToken)
    {
        var receipt = await _db.StockReceipts.AsNoTracking()
            .Include(x => x.Lines)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (receipt is null)
        {
            return null;
        }

        return new StockReceiptDetailDto
        {
            Id = receipt.Id,
            DocumentNo = receipt.DocumentNo,
            ReceivedAtUtc = receipt.ReceivedAtUtc,
            Supplier = receipt.Supplier,
            Note = receipt.Note,
            TotalAmount = receipt.TotalAmount,
            Lines = receipt.Lines
                .OrderBy(x => x.Id)
                .Select(x => new StockReceiptDetailLineDto(
                    x.ProductId,
                    x.Product?.Sku ?? string.Empty,
                    x.Product?.Name ?? string.Empty,
                    x.Quantity,
                    x.UnitCost,
                    x.LineTotal))
                .ToList()
        };
    }

    private async Task<StockIssueDetailDto?> BuildIssueDetailAsync(int id, CancellationToken cancellationToken)
    {
        var issue = await _db.StockIssues.AsNoTracking()
            .Include(x => x.Lines)
            .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (issue is null)
        {
            return null;
        }

        return new StockIssueDetailDto
        {
            Id = issue.Id,
            DocumentNo = issue.DocumentNo,
            IssuedAtUtc = issue.IssuedAtUtc,
            Customer = issue.Customer,
            Note = issue.Note,
            TotalAmount = issue.TotalAmount,
            Lines = issue.Lines
                .OrderBy(x => x.Id)
                .Select(x => new StockIssueDetailLineDto(
                    x.ProductId,
                    x.Product?.Sku ?? string.Empty,
                    x.Product?.Name ?? string.Empty,
                    x.Quantity,
                    x.UnitCost,
                    x.LineTotal))
                .ToList()
        };
    }

    private static void ValidateReceiptRequest(CreateStockReceiptRequest request)
    {
        if (request.Lines.Count == 0)
        {
            throw new InventoryValidationException("Receipt must contain at least one line.");
        }

        if (request.Lines.Any(x => x.Quantity <= 0))
        {
            throw new InventoryValidationException("Receipt quantities must be greater than zero.");
        }

        if (request.Lines.Any(x => x.UnitCost < 0))
        {
            throw new InventoryValidationException("Receipt unit cost cannot be negative.");
        }
    }

    private static void ValidateIssueRequest(CreateStockIssueRequest request)
    {
        if (request.Lines.Count == 0)
        {
            throw new InventoryValidationException("Issue must contain at least one line.");
        }

        if (request.Lines.Any(x => x.Quantity <= 0))
        {
            throw new InventoryValidationException("Issue quantities must be greater than zero.");
        }
    }
}
