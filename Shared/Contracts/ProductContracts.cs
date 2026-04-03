using System.ComponentModel.DataAnnotations;

namespace MyApp.Shared.Contracts;

public record ProductDto(
    int Id,
    string Sku,
    string Name,
    string? Description,
    int CategoryId,
    string CategoryName,
    int OnHandQty,
    decimal AverageCost,
    int ReorderLevel,
    bool IsActive,
    bool IsDeleted,
    DateTime LastUpdatedUtc);

public class CreateProductRequest
{
    [Required]
    [MaxLength(50)]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; } = 5;
}

public class UpdateProductRequest : CreateProductRequest
{
    public bool IsActive { get; set; } = true;
}
