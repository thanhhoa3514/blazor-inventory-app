using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyApp.Server.Auth;
using MyApp.Shared.Contracts;

namespace MyApp.Server.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<UserSessionDto>> Login(LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var normalized = request.UserNameOrEmail.Trim();
        var user = await _userManager.FindByNameAsync(normalized)
            ?? await _userManager.FindByEmailAsync(normalized);

        if (user is null)
        {
            return Unauthorized("Invalid username/email or password.");
        }

        var result = await _signInManager.PasswordSignInAsync(user.UserName!, request.Password, isPersistent: true, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return Unauthorized("Invalid username/email or password.");
        }

        return Ok(await BuildSessionAsync(user));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserSessionDto>> GetCurrentUser()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(await BuildSessionAsync(user));
    }

    private async Task<UserSessionDto> BuildSessionAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        return new UserSessionDto
        {
            IsAuthenticated = true,
            UserName = user.UserName,
            DisplayName = user.DisplayName,
            Roles = roles.OrderBy(x => x).ToList()
        };
    }
}
