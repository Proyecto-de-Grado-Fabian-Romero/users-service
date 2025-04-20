using UsersService.Src.Domain.Enums;

namespace UsersService.Src.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public Guid PublicId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public bool VerifiedEmail { get; set; }

    public bool VerifiedPhone { get; set; }

    public UserRole Role { get; set; }

    public string? PhotoFileId { get; set; }

    public string? PhotoFileName { get; set; }

    public string? PhotoFileUrl { get; set; }

    public bool? Verified { get; set; }
}
