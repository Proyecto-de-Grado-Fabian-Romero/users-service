using UsersService.Src.Infraestructure.Messaging.Contracts;
using UsersService.Src.Infrastructure.Messaging.Contracts;

namespace UsersService.Src.Infraestructure.Messaging;

public interface INotificationsPublisher
{
    Task PublishTokenUpsertAsync(TokenUpsertMessage m, CancellationToken ct = default);

    Task PublishSessionEndedAsync(SessionEndedMessage m, CancellationToken ct = default);
}
