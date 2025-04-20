namespace UsersService.src.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using UsersService.Src.Application.DTOs;
using UsersService.Src.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

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
}
