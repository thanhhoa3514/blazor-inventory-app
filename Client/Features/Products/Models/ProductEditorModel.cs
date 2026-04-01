using MyApp.Shared.Contracts;

namespace MyApp.Client.Features.Products.Models;

public sealed class ProductEditorModel
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public int ReorderLevel { get; set; } = 5;
    public bool IsActive { get; set; } = true;

    public CreateProductRequest ToCreateRequest() => new()
    {
        Sku = Sku,
        Name = Name,
        Description = Description,
        CategoryId = CategoryId,
        ReorderLevel = ReorderLevel
    };

    public UpdateProductRequest ToUpdateRequest() => new()
    {
        Sku = Sku,
        Name = Name,
        Description = Description,
        CategoryId = CategoryId,
        ReorderLevel = ReorderLevel,
        IsActive = IsActive
    };

    public void Reset()
    {
        Sku = string.Empty;
        Name = string.Empty;
        Description = null;
        CategoryId = 0;
        ReorderLevel = 5;
        IsActive = true;
    }

    public void Load(ProductDto item)
    {
        Sku = item.Sku;
        Name = item.Name;
        Description = item.Description;
        CategoryId = item.CategoryId;
        ReorderLevel = item.ReorderLevel;
        IsActive = item.IsActive;
    }
}
