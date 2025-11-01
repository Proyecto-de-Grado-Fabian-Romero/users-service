namespace UsersService.Src.Application.DTOs;

public class ConfirmForgotPasswordRequest
{
    public string Email { get; set; } = default!;

    public string Code { get; set; } = default!;

    public string NewPassword { get; set; } = default!;
}
