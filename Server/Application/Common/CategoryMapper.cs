using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Common;

public static class CategoryMapper
{
    public static CategoryDto ToDto(this Category entity)
        => new(entity.Id, entity.Name, entity.Description, entity.CreatedAtUtc);
}
