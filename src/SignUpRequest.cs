namespace UsersService.src;

public class SignUpRequest
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string UserId { get; set; } = Guid.NewGuid().ToString();

    public string Phone { get; set; } = string.Empty;
}
