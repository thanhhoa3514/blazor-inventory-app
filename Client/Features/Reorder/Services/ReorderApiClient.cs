using System.Net.Http.Json;
using MyApp.Shared.Contracts;

namespace MyApp.Client.Features.Reorder.Services;

public sealed class ReorderApiClient
{
    private readonly HttpClient _httpClient;

    public ReorderApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ReorderRecommendationDto>> GetRecommendationsAsync(
        string? search = null,
        int? categoryId = null,
        string? priority = null,
        CancellationToken cancellationToken = default)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(search)) query.Add($"search={Uri.EscapeDataString(search)}");
        if (categoryId.HasValue && categoryId.Value > 0) query.Add($"categoryId={categoryId.Value}");
        if (!string.IsNullOrWhiteSpace(priority)) query.Add($"priority={Uri.EscapeDataString(priority)}");

        var path = "api/inventory/reorder-recommendations";
        if (query.Count > 0) path += "?" + string.Join("&", query);

        return await _httpClient.GetFromJsonAsync<List<ReorderRecommendationDto>>(path, cancellationToken) ?? [];
    }
}
