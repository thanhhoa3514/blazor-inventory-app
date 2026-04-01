using Microsoft.AspNetCore.Identity;

namespace MyApp.Server.Auth;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
