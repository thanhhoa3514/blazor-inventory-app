using System.Net.Http.Json;
using System.Globalization;
using MyApp.Shared.Contracts;

namespace MyApp.Client.Features.Inventory.Services;

public sealed class InventoryApiClient
{
    private readonly HttpClient _httpClient;

    public InventoryApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<InventorySummaryDto?> GetSummaryAsync(CancellationToken cancellationToken = default)
        => await _httpClient.GetFromJsonAsync<InventorySummaryDto>("api/inventory/summary", cancellationToken);

    public async Task<ProductStockCardDto?> GetProductStockCardAsync(
        int productId,
        DateTime? fromDate,
        DateTime? toDate,
        string? movementType,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>();

        if (fromDate.HasValue)
            query.Add($"fromUtc={Uri.EscapeDataString(fromDate.Value.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))}");

        if (toDate.HasValue)
            query.Add($"toUtc={Uri.EscapeDataString(toDate.Value.ToUniversalTime().Date.AddDays(1).AddTicks(-1).ToString("O", CultureInfo.InvariantCulture))}");

        if (!string.IsNullOrWhiteSpace(movementType))
            query.Add($"movementType={Uri.EscapeDataString(movementType)}");

        var path = $"api/inventory/products/{productId}/stock-card";
        if (query.Count > 0)
            path = $"{path}?{string.Join("&", query)}";

        var response = await _httpClient.GetAsync(path, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProductStockCardDto>(cancellationToken);
    }
}
