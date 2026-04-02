using System.Net.Http.Json;
using MyApp.Client.Shared.Models;
using MyApp.Client.Shared.Services;
using MyApp.Shared.Contracts;

namespace MyApp.Client.Features.Suppliers.Services;

public sealed class SupplierApiClient
{
    private readonly HttpClient _httpClient;

    public SupplierApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<SupplierDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _httpClient.GetFromJsonAsync<List<SupplierDto>>("api/suppliers", cancellationToken) ?? [];

    public async Task<List<SupplierDto>> GetActiveAsync(CancellationToken cancellationToken = default)
        => (await GetAllAsync(cancellationToken)).Where(x => x.IsActive).ToList();

    public async Task<ApiCommandResult> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/suppliers", request, cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }

    public async Task<ApiCommandResult> UpdateAsync(int id, UpdateSupplierRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/suppliers/{id}", request, cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }

    public async Task<ApiCommandResult> DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/suppliers/{id}", cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }
}
