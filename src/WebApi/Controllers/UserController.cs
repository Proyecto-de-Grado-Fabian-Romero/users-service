using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Application.Interfaces;

namespace UsersService.src.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("{publicId}")]
    public async Task<ActionResult<UserDTO>> GetUserByPublicId(Guid publicId)
    {
        var user = await _userService.GetByPublicIdAsync(publicId);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.LoginAsync(request.Email, request.Password);
        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }

        Response.Cookies.Append("accessToken", user.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(1),
        });

        Response.Cookies.Append("refreshToken", user.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(100),
        });

        Response.Cookies.Append("publicId", user.PublicId.ToString(), new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(100),
        });

        return Ok(user);
    }

    [HttpGet("me")]
    public async Task<ActionResult<LoggedUserDTO>> GetCurrentUser()
    {
        var accessToken = Request.Cookies["accessToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized("No access token provided.");
        }

        var user = await _userService.GetUserFromAccessTokenAsync(accessToken);
        return user == null ? Unauthorized() : Ok(user);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult> RefreshAccessToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("No refresh token provided.");
        }

        var newAccessToken = await _userService.RefreshAccessTokenAsync(refreshToken);
        if (string.IsNullOrEmpty(newAccessToken))
        {
            return Unauthorized("Invalid refresh token.");
        }

        Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(1),
        });

        return Ok(new { message = "Access token refreshed." });
    }

    [HttpGet("verify-token")]
    public async Task<ActionResult<bool>> VerifyAccessToken()
    {
        var accessToken = Request.Cookies["accessToken"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return Ok(false);
        }

        var isValid = await _userService.IsAccessTokenValidAsync(accessToken);
        return Ok(isValid);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        await _userService.LogoutAsync(refreshToken);

        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");
        Response.Cookies.Delete("publicId");

        return Ok(new { message = "Logged out successfully." });
    }
}
