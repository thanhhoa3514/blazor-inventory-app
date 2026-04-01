using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using MyApp.Client.Features.Auth.Services;
using MyApp.Shared.Contracts;

namespace MyApp.Client.Features.Auth.State;

public sealed class AuthStateProvider : AuthenticationStateProvider
{
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());
    private readonly AuthApiClient _authApiClient;

    public AuthStateProvider(AuthApiClient authApiClient)
    {
        _authApiClient = authApiClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var session = await _authApiClient.GetCurrentUserAsync();
        return new AuthenticationState(BuildPrincipal(session));
    }

    public async Task<(bool Success, string? ErrorMessage)> SignInAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _authApiClient.SignInAsync(request, cancellationToken);
        if (!result.Success)
        {
            return (false, result.ErrorMessage);
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(BuildPrincipal(result.Session))));
        return (true, null);
    }

    public async Task SignOutAsync(CancellationToken cancellationToken = default)
    {
        if (await _authApiClient.SignOutAsync(cancellationToken))
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(Anonymous)));
        }
    }

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        var session = await _authApiClient.GetCurrentUserAsync(cancellationToken);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(BuildPrincipal(session))));
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
