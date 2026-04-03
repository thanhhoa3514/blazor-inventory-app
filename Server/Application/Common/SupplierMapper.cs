using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Common;

public static class SupplierMapper
{
    public static SupplierDto ToDto(this Supplier entity)
        => new(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.IsActive,
            entity.IsDeleted,
            entity.CreatedAtUtc,
            entity.LastUpdatedUtc);
}
