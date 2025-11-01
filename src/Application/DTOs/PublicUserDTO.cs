using UsersService.Src.Application.DTOs.BankPaymentData;

namespace UsersService.Src.Application.DTOs;

public class UserDTO
{
    public Guid PublicId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public bool VerifiedEmail { get; set; }

    public bool VerifiedPhone { get; set; }

    required public string Role { get; set; }

    public string? PhotoFileUrl { get; set; }

    public bool? Verified { get; set; }

    public BankPaymentDataDTO? BankPaymentData { get; set; }
}
