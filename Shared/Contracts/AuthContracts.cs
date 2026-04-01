using System.ComponentModel.DataAnnotations;

namespace MyApp.Shared.Contracts;

public class LoginRequest
{
    [Required]
    [MaxLength(100)]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;
}

public class UserSessionDto
{
    public bool IsAuthenticated { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public List<string> Roles { get; set; } = new();
}
