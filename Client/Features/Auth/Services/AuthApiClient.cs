using System.Net.Http.Json;
using MyApp.Client.Shared.Services;
using MyApp.Shared.Contracts;

namespace MyApp.Client.Features.Auth.Services;

public sealed class AuthApiClient
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserSessionDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserSessionDto>("api/auth/me", cancellationToken);
        }
        catch
        {
            return null;
        }
    }

    public async Task<(bool Success, string? ErrorMessage, UserSessionDto? Session)> SignInAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return (false, await response.ReadErrorMessageAsync("Login failed."), null);
        }

        var session = await response.Content.ReadFromJsonAsync<UserSessionDto>(cancellationToken);
        return (true, null, session);
    }

    public async Task<bool> SignOutAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync("api/auth/logout", content: null, cancellationToken);
        return response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Unauthorized;
    }
}
