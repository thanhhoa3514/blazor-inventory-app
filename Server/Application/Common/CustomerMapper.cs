using MyApp.Shared.Contracts;
using MyApp.Shared.Domain;

namespace MyApp.Server.Application.Common;

public static class CustomerMapper
{
    public static CustomerDto ToDto(this Customer entity)
        => new(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.IsActive,
            entity.CreatedAtUtc,
            entity.LastUpdatedUtc);
}
