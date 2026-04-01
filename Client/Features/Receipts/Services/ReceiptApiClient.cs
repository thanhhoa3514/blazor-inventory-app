using System.Net.Http.Json;
using MyApp.Client.Shared.Models;
using MyApp.Client.Shared.Services;
using MyApp.Shared.Contracts;

namespace MyApp.Client.Features.Receipts.Services;

public sealed class ReceiptApiClient
{
    private readonly HttpClient _httpClient;

    public ReceiptApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<StockReceiptListDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _httpClient.GetFromJsonAsync<List<StockReceiptListDto>>("api/receipts", cancellationToken) ?? [];

    public async Task<StockReceiptDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/receipts/{id}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StockReceiptDetailDto>(cancellationToken);
    }

    public async Task<ApiCommandResult> CreateAsync(CreateStockReceiptRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/receipts", request, cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }
}
