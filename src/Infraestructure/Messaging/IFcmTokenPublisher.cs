using UsersService.Src.Infraestructure.Messaging.Contracts;

namespace UsersService.Src.Infraestructure.Messaging;

public interface IFcmTokenPublisher
{
    Task PublishAsync(TokenUpsertMessage message, CancellationToken ct = default);
}
