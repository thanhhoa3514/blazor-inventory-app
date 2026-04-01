using System.Net.Http.Json;
using MyApp.Client.Shared.Models;
using MyApp.Client.Shared.Services;
using MyApp.Shared.Contracts;

namespace MyApp.Client.Features.Adjustments.Services;

public sealed class AdjustmentApiClient
{
    private readonly HttpClient _httpClient;

    public AdjustmentApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<StockAdjustmentListDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _httpClient.GetFromJsonAsync<List<StockAdjustmentListDto>>("api/adjustments", cancellationToken) ?? [];

    public async Task<StockAdjustmentDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/adjustments/{id}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StockAdjustmentDetailDto>(cancellationToken);
    }

    public async Task<ApiCommandResult> CreateAsync(CreateStockAdjustmentRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/adjustments", request, cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }
}
