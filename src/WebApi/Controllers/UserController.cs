namespace UsersService.src.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet("{publicId}")]
    public async Task<ActionResult<UserDTO>> GetUserByPublicId(Guid publicId)
    {
        var user = await _userService.GetByPublicIdAsync(publicId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoggedUserDTO>> Login([FromBody] Src.Application.DTOs.LoginRequest request)
    {
        var user = await _userService.LoginAsync(request.Email, request.Password);
        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }

        return Ok(user);
    }

    [HttpGet("me")]
    public async Task<ActionResult<LoggedUserDTO>> GetCurrentUser()
    {
        var accessToken = Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);
        var user = await _userService.GetUserFromAccessTokenAsync(accessToken);
        return user == null ? Unauthorized() : Ok(user);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<string>> RefreshAccessToken([FromBody] RefreshTokenRequest request)
    {
        var newToken = await _userService.RefreshAccessTokenAsync(request.RefreshToken);
        return string.IsNullOrEmpty(newToken) ? Unauthorized() : Ok(newToken);
    }

    [HttpGet("verify-token")]
    public async Task<ActionResult<bool>> VerifyAccessToken()
    {
        var accessToken = Request.Headers.Authorization.ToString().Replace("Bearer ", string.Empty);
        var isValid = await _userService.IsAccessTokenValidAsync(accessToken);
        return Ok(isValid);
    }
}
