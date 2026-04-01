using System.Security.Claims;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using MyApp.Shared.Contracts;

namespace MyApp.Client.Auth;

public class ServerAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    public ServerAuthenticationStateProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var session = await TryGetCurrentUserAsync();
        return new AuthenticationState(BuildPrincipal(session));
    }

    public async Task<(bool Success, string? Error)> SignInAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(error) ? "Login failed." : error.Trim().Trim('"'));
        }

        var session = await response.Content.ReadFromJsonAsync<UserSessionDto>();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(BuildPrincipal(session))));
        return (true, null);
    }

    public async Task SignOutAsync()
    {
        var response = await _httpClient.PostAsync("api/auth/logout", null);
        if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(Anonymous)));
        }
    }

    public async Task RefreshAsync()
    {
        var session = await TryGetCurrentUserAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(BuildPrincipal(session))));
    }

    private async Task<UserSessionDto?> TryGetCurrentUserAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserSessionDto>("api/auth/me");
        }
        catch
        {
            return null;
        }
    }

    private static ClaimsPrincipal BuildPrincipal(UserSessionDto? session)
    {
        if (session?.IsAuthenticated != true || string.IsNullOrWhiteSpace(session.UserName))
        {
            return Anonymous;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, session.UserName)
        };

        if (!string.IsNullOrWhiteSpace(session.DisplayName))
        {
            claims.Add(new Claim("display_name", session.DisplayName));
        }

        foreach (var role in session.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "ServerCookie"));
    }
}
