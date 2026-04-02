using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Inventory.Commands;

public sealed class CreateReceiptCommand
{
    private readonly IInventoryUnitOfWork _uow;
    private readonly IProductRepository _products;
    private readonly ISupplierRepository _suppliers;
    private readonly IStockReceiptRepository _receipts;
    private readonly ILogger<CreateReceiptCommand> _logger;

    public CreateReceiptCommand(
        IInventoryUnitOfWork uow,
        IProductRepository products,
        ISupplierRepository suppliers,
        IStockReceiptRepository receipts,
        ILogger<CreateReceiptCommand> logger)
    {
        _uow = uow;
        _products = products;
        _suppliers = suppliers;
        _receipts = receipts;
        _logger = logger;
    }

    public async Task<AppResult<StockReceiptDetailDto>> ExecuteAsync(
        CreateStockReceiptRequest request,
        CancellationToken ct = default)
    {
        var validationError = ValidateRequest(request);
        if (validationError is not null)
            return new AppResult<StockReceiptDetailDto>.ValidationError(validationError);

        var now = DateTime.UtcNow;
        var documentNo = $"RCPT-{now:yyyyMMddHHmmssfff}";

        await _uow.BeginTransactionAsync(ct);
        try
        {
            Supplier? supplier = null;
            if (request.SupplierId.HasValue)
            {
                supplier = await _suppliers.FindActiveAsync(request.SupplierId.Value, ct);
                if (supplier is null)
                {
                    await _uow.RollbackAsync(ct);
                    return new AppResult<StockReceiptDetailDto>.ValidationError(
                        $"Supplier with ID {request.SupplierId.Value} does not exist or is inactive.");
                }
            }

            var receipt = new StockReceipt
            {
                DocumentNo = documentNo,
                SupplierId = supplier?.Id,
                Note = request.Note?.Trim(),
                ReceivedAtUtc = now
            };

            decimal total = 0m;
            foreach (var line in request.Lines)
            {
                var product = await _products.FindActiveAsync(line.ProductId, ct);
                if (product is null)
                {
                    await _uow.RollbackAsync(ct);
                    return new AppResult<StockReceiptDetailDto>.ValidationError(
                        $"Product with ID {line.ProductId} does not exist or is inactive.");
                }

                var lineTotal = Math.Round(line.Quantity * line.UnitCost, 2, MidpointRounding.AwayFromZero);
                var currentValue = Math.Round(product.OnHandQty * product.AverageCost, 2, MidpointRounding.AwayFromZero);
                var newQty = product.OnHandQty + line.Quantity;
                var newAverage = newQty == 0
                    ? 0
                    : Math.Round((currentValue + lineTotal) / newQty, 2, MidpointRounding.AwayFromZero);

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

                _uow.AddLedgerEntry(new InventoryLedgerEntry
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
            _uow.AddReceipt(receipt);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);

            _logger.LogInformation("Created receipt {DocumentNo} with {LineCount} lines and total {TotalAmount}.",
                receipt.DocumentNo, receipt.Lines.Count, receipt.TotalAmount);

            var dto = await _receipts.GetByIdAsync(receipt.Id, ct)
                ?? throw new InvalidOperationException("Receipt created but could not be loaded.");
            return new AppResult<StockReceiptDetailDto>.Ok(dto);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            await _uow.RollbackAsync(ct);
            _logger.LogError(ex, "Failed to create stock receipt.");
            throw;
        }
    }

    private static string? ValidateRequest(CreateStockReceiptRequest request)
    {
        if (request.Lines.Count == 0)
            return "Receipt must contain at least one line.";
        if (request.Lines.Any(x => x.Quantity <= 0))
            return "Receipt quantities must be greater than zero.";
        if (request.Lines.Any(x => x.UnitCost < 0))
            return "Receipt unit cost cannot be negative.";
        return null;
    }
}
