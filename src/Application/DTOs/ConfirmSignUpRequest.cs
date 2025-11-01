namespace UsersService.Src.Application.DTOs;

public class ConfirmSignUpRequest
{
    public string Email { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;
}
