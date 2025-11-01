namespace UsersService.Src.Infrastructure.Messaging.Contracts;

public sealed class SessionEndedMessage
{
    public Guid UserPublicId { get; set; }

    public string SessionId { get; set; } = string.Empty;
}
