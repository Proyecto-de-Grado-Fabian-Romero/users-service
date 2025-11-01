namespace UsersService.Src.Infraestructure.Messaging.Contracts;

public sealed class TokenUpsertMessage
{
    public Guid UserPublicId { get; set; }

    public string Token { get; set; } = string.Empty;

    public string SessionId { get; set; } = string.Empty;

    public string? DeviceInfo { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }
}
