using System.ComponentModel.DataAnnotations;

namespace MyApp.Shared.Contracts;

public record CustomerDto(
    int Id,
    string Name,
    string? Description,
    bool IsActive,
    bool IsDeleted,
    DateTime CreatedAtUtc,
    DateTime LastUpdatedUtc);

public class CreateCustomerRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class UpdateCustomerRequest : CreateCustomerRequest
{
    public bool IsActive { get; set; } = true;
}
