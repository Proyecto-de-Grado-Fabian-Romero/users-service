using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UsersService.Src.Infraestructure.Messaging.Contracts;
using UsersService.Src.Infrastructure.Messaging.Contracts;

namespace UsersService.Src.Infraestructure.Messaging;

public sealed class RabbitMqNotificationsPublisher : INotificationsPublisher, IDisposable
{
    private readonly RabbitMqOptions _opt;
    private readonly IConnection _conn;
    private readonly IModel _ch;

    public RabbitMqNotificationsPublisher(IOptions<RabbitMqOptions> opt)
    {
        _opt = opt.Value;
        var factory = new ConnectionFactory { Uri = new Uri(_opt.ConnectionString) };
        _conn = factory.CreateConnection();
        _ch = _conn.CreateModel();

        _ch.ExchangeDeclare(_opt.Exchange, ExchangeType.Direct, durable: true, autoDelete: false);

        _ch.QueueDeclare(
            "notifications.token.upsert",
            durable: true,
            exclusive: false,
            autoDelete: false
        );
        _ch.QueueBind("notifications.token.upsert", _opt.Exchange, "token.upsert");

        _ch.QueueDeclare(
            "notifications.session.ended",
            durable: true,
            exclusive: false,
            autoDelete: false
        );
        _ch.QueueBind("notifications.session.ended", _opt.Exchange, "session.ended");
        Console.WriteLine("✅ RabbitMQ connected and queues declared");
    }

    public Task PublishTokenUpsertAsync(TokenUpsertMessage m, CancellationToken ct = default)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(m);
        var props = _ch.CreateBasicProperties();
        props.DeliveryMode = 2;
        _ch.BasicPublish(_opt.Exchange, "token.upsert", props, body);
        Console.WriteLine("✅ Published token upsert message");
        return Task.CompletedTask;
    }

    public Task PublishSessionEndedAsync(SessionEndedMessage m, CancellationToken ct = default)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(m);
        var props = _ch.CreateBasicProperties();
        props.DeliveryMode = 2;
        _ch.BasicPublish(_opt.Exchange, "session.ended", props, body);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _ch?.Dispose();
        _conn?.Dispose();
    }
}
