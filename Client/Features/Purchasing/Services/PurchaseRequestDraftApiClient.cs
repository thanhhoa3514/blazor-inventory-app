using System.Net.Http.Json;
using MyApp.Client.Shared.Models;
using MyApp.Client.Shared.Services;
using MyApp.Shared.Contracts;

namespace MyApp.Client.Features.Purchasing.Services;

public sealed class PurchaseRequestDraftApiClient
{
    private readonly HttpClient _httpClient;

    public PurchaseRequestDraftApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PurchaseRequestDraftListDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _httpClient.GetFromJsonAsync<List<PurchaseRequestDraftListDto>>("api/purchase-request-drafts", cancellationToken) ?? [];

    public async Task<PurchaseRequestDraftDetailDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"api/purchase-request-drafts/{id}", cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PurchaseRequestDraftDetailDto>(cancellationToken);
    }

    public async Task<PurchaseRequestDraftDetailDto?> CreateAsync(CreatePurchaseRequestDraftRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/purchase-request-drafts", request, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<PurchaseRequestDraftDetailDto>(cancellationToken);
    }

    public async Task<ApiCommandResult> UpdateLineAsync(int draftId, int lineId, UpdatePurchaseRequestDraftLineRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/purchase-request-drafts/{draftId}/lines/{lineId}", request, cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }

    public async Task<ApiCommandResult> RemoveLineAsync(int draftId, int lineId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/purchase-request-drafts/{draftId}/lines/{lineId}", cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }

    public async Task<PurchaseRequestDraftDetailDto?> PrepareAsync(int draftId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync($"api/purchase-request-drafts/{draftId}/prepare", null, cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<PurchaseRequestDraftDetailDto>(cancellationToken);
    }
}
