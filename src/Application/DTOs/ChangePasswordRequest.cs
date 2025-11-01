namespace UsersService.Src.Application.DTOs;

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = default!;

    public string NewPassword { get; set; } = default!;
}
