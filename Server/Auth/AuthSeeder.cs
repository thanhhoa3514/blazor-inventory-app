using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MyApp.Shared.Security;

namespace MyApp.Server.Auth;

public static class AuthSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AuthSeedOptions>>();

        foreach (var role in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        foreach (var seedUser in options.Value.Users)
        {
            var user = await userManager.FindByNameAsync(seedUser.UserName)
                ?? await userManager.FindByEmailAsync(seedUser.Email);

            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = seedUser.UserName,
                    Email = seedUser.Email,
                    DisplayName = seedUser.DisplayName,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, seedUser.Password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(x => x.Description));
                    throw new InvalidOperationException($"Failed to seed user '{seedUser.UserName}': {errors}");
                }
            }
            else if (user.DisplayName != seedUser.DisplayName)
            {
                user.DisplayName = seedUser.DisplayName;
                await userManager.UpdateAsync(user);
            }

            var currentRoles = await userManager.GetRolesAsync(user);
            var missingRoles = seedUser.Roles.Except(currentRoles).ToList();
            if (missingRoles.Count > 0)
            {
                var roleResult = await userManager.AddToRolesAsync(user, missingRoles);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join("; ", roleResult.Errors.Select(x => x.Description));
                    throw new InvalidOperationException($"Failed to assign roles to '{seedUser.UserName}': {errors}");
                }
            }
        }
    }
}
