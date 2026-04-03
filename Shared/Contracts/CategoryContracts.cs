using System.ComponentModel.DataAnnotations;

namespace MyApp.Shared.Contracts;

public record CategoryDto(int Id, string Name, string? Description, bool IsDeleted, DateTime CreatedAtUtc);

public class CreateCategoryRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }
}

public class UpdateCategoryRequest : CreateCategoryRequest;
