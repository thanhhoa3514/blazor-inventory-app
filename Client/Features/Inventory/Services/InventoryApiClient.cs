using System.Net.Http.Json;
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
}
