using System.Net.Http.Json;
using MyApp.Client.Shared.Models;
using MyApp.Client.Shared.Services;
using MyApp.Shared.Contracts;

namespace MyApp.Client.Features.Customers.Services;

public sealed class CustomerApiClient
{
    private readonly HttpClient _httpClient;

    public CustomerApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CustomerDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _httpClient.GetFromJsonAsync<List<CustomerDto>>("api/customers", cancellationToken) ?? [];

    public async Task<List<CustomerDto>> GetDeletedAsync(CancellationToken cancellationToken = default)
        => await _httpClient.GetFromJsonAsync<List<CustomerDto>>("api/customers/deleted", cancellationToken) ?? [];

    public async Task<List<CustomerDto>> GetActiveAsync(CancellationToken cancellationToken = default)
        => (await GetAllAsync(cancellationToken)).Where(x => x.IsActive).ToList();

    public async Task<ApiCommandResult> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/customers", request, cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }

    public async Task<ApiCommandResult> UpdateAsync(int id, UpdateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/customers/{id}", request, cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }

    public async Task<ApiCommandResult> DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync($"api/customers/{id}/deactivate", null, cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }

    public async Task<ApiCommandResult> SoftDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync($"api/customers/{id}/soft-delete", null, cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }

    public async Task<ApiCommandResult> RestoreAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync($"api/customers/{id}/restore", null, cancellationToken);
        return response.IsSuccessStatusCode
            ? ApiCommandResult.Ok()
            : ApiCommandResult.Fail(await response.ReadErrorMessageAsync());
    }
}
