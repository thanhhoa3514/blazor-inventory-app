using MyApp.Server.Application.Common;
using MyApp.Server.Persistence.Repositories;
using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Inventory.Commands;

public sealed class CreateAdjustmentCommand
{
    private readonly IInventoryUnitOfWork _uow;
    private readonly IProductRepository _products;
    private readonly IStockAdjustmentRepository _adjustments;
    private readonly ILogger<CreateAdjustmentCommand> _logger;

    public CreateAdjustmentCommand(
        IInventoryUnitOfWork uow,
        IProductRepository products,
        IStockAdjustmentRepository adjustments,
        ILogger<CreateAdjustmentCommand> logger)
    {
        _uow = uow;
        _products = products;
        _adjustments = adjustments;
        _logger = logger;
    }

    public async Task<AppResult<StockAdjustmentDetailDto>> ExecuteAsync(
        CreateStockAdjustmentRequest request,
        CancellationToken ct = default)
    {
        var validationError = ValidateRequest(request);
        if (validationError is not null)
            return new AppResult<StockAdjustmentDetailDto>.ValidationError(validationError);

        var now = DateTime.UtcNow;
        var documentNo = $"ADJ-{now:yyyyMMddHHmmssfff}";

        await _uow.BeginTransactionAsync(ct);
        try
        {
            var adjustment = new StockAdjustment
            {
                DocumentNo = documentNo,
                Reason = request.Reason.Trim(),
                Note = request.Note?.Trim(),
                AdjustedAtUtc = now
            };

            decimal total = 0m;
            foreach (var line in request.Lines)
            {
                var normalizedDirection = NormalizeDirection(line.Direction);
                if (normalizedDirection is null)
                {
                    await _uow.RollbackAsync(ct);
                    return new AppResult<StockAdjustmentDetailDto>.ValidationError(
                        "Adjustment direction must be either increase or decrease.");
                }

                var product = await _products.FindActiveAsync(line.ProductId, ct);
                if (product is null)
                {
                    await _uow.RollbackAsync(ct);
                    return new AppResult<StockAdjustmentDetailDto>.ValidationError(
                        $"Product with ID {line.ProductId} does not exist or is inactive.");
                }

                var signedQuantity = normalizedDirection == StockAdjustmentDirections.Increase
                    ? line.Quantity
                    : -line.Quantity;

                if (product.OnHandQty + signedQuantity < 0)
                {
                    await _uow.RollbackAsync(ct);
                    return new AppResult<StockAdjustmentDetailDto>.ValidationError(
                        $"Insufficient stock for {product.Sku}. On hand {product.OnHandQty}, requested adjustment {signedQuantity}.");
                }

                var unitCost = product.AverageCost;
                var lineTotal = Math.Round(signedQuantity * unitCost, 2, MidpointRounding.AwayFromZero);
                product.OnHandQty += signedQuantity;
                product.LastUpdatedUtc = now;

                adjustment.Lines.Add(new StockAdjustmentLine
                {
                    ProductId = product.Id,
                    Direction = normalizedDirection,
                    Quantity = line.Quantity,
                    UnitCost = unitCost,
                    LineTotal = lineTotal
                });

                _uow.AddLedgerEntry(new InventoryLedgerEntry
                {
                    ProductId = product.Id,
                    MovementType = "ADJUSTMENT",
                    ReferenceNo = documentNo,
                    OccurredAtUtc = now,
                    QuantityChange = signedQuantity,
                    UnitCost = unitCost,
                    ValueChange = lineTotal,
                    RunningOnHandQty = product.OnHandQty,
                    RunningAverageCost = product.AverageCost
                });

                total += lineTotal;
            }

            adjustment.TotalAmount = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            _uow.AddAdjustment(adjustment);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitAsync(ct);

            _logger.LogInformation("Created adjustment {DocumentNo} with {LineCount} lines and net total {TotalAmount}.",
                adjustment.DocumentNo, adjustment.Lines.Count, adjustment.TotalAmount);

            var dto = await _adjustments.GetByIdAsync(adjustment.Id, ct)
                ?? throw new InvalidOperationException("Adjustment created but could not be loaded.");
            return new AppResult<StockAdjustmentDetailDto>.Ok(dto);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            await _uow.RollbackAsync(ct);
            _logger.LogError(ex, "Failed to create stock adjustment.");
            throw;
        }
    }

    private static string? ValidateRequest(CreateStockAdjustmentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            return "Adjustment reason is required.";
        if (request.Lines.Count == 0)
            return "Adjustment must contain at least one line.";
        if (request.Lines.Any(x => x.Quantity <= 0))
            return "Adjustment quantities must be greater than zero.";
        return null;
    }

    private static string? NormalizeDirection(string? direction)
    {
        if (string.Equals(direction, StockAdjustmentDirections.Increase, StringComparison.OrdinalIgnoreCase))
            return StockAdjustmentDirections.Increase;
        if (string.Equals(direction, StockAdjustmentDirections.Decrease, StringComparison.OrdinalIgnoreCase))
            return StockAdjustmentDirections.Decrease;
        return null;
    }
}
