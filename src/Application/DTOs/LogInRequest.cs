namespace UsersService.Src.Application.DTOs;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string? FcmToken { get; set; }

    public string? DeviceInfo { get; set; }
}
