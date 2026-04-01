namespace MyApp.Server.Auth;

public class AuthSeedOptions
{
    public List<SeedUserOptions> Users { get; set; } = new();
}

public class SeedUserOptions
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
