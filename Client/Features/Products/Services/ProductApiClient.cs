using System.Net.Http.Json;
using MyApp.Client.Shared.Models;
using MyApp.Client.Shared.Services;
using MyApp.Shared.Contracts;

namespace MyApp.Client.Features.Products.Services;

public sealed class ProductApiClient
{
    private readonly HttpClient _httpClient;

    public ProductApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/products", cancellationToken) ?? [];

    public async Task<List<ProductDto>> GetDeletedAsync(CancellationToken cancellationToken = default)
        => await _httpClient.GetFromJsonAsync<List<ProductDto>>("api/products/deleted", cancellationToken) ?? [];

    public async Task<List<ProductDto>> GetActiveAsync(CancellationToken cancellationToken = default)
        => (await GetAllAsync(cancellationToken)).Where(x => x.IsActive).ToList();

    public async Task<ApiCommandResult> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/products", request, cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }

    public async Task<ApiCommandResult> UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", request, cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }

    public async Task<ApiCommandResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/products/{id}", cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }

    public async Task<ApiCommandResult> RestoreAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync($"api/products/{id}/restore", null, cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }
}
