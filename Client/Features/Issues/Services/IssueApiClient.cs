using System.Net.Http.Json;
using MyApp.Client.Shared.Models;
using MyApp.Client.Shared.Services;
using MyApp.Shared.Contracts;

namespace MyApp.Client.Features.Issues.Services;

public sealed class IssueApiClient
{
    private readonly HttpClient _httpClient;

    public IssueApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<StockIssueListDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _httpClient.GetFromJsonAsync<List<StockIssueListDto>>("api/issues", cancellationToken) ?? [];

    public async Task<StockIssueDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/issues/{id}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StockIssueDetailDto>(cancellationToken);
    }

    public async Task<ApiCommandResult> CreateAsync(CreateStockIssueRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/issues", request, cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }
}
