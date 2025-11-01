using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using UsersService.Src.Infraestructure.Messaging.Contracts;

namespace UsersService.Src.Infraestructure.Messaging;

public sealed class RabbitMqFcmTokenPublisher : IFcmTokenPublisher, IDisposable
{
    private readonly RabbitMqOptions _opt;
    private readonly IConnection _conn;
    private readonly IModel _ch;

    public RabbitMqFcmTokenPublisher(IOptions<RabbitMqOptions> opt)
    {
        _opt = opt.Value;
        var factory = new ConnectionFactory { Uri = new Uri(_opt.ConnectionString) };
        _conn = factory.CreateConnection();
        _ch = _conn.CreateModel();
        _ch.ExchangeDeclare(_opt.Exchange, ExchangeType.Direct, durable: true, autoDelete: false);
        _ch.QueueDeclare(durable: true, exclusive: false, autoDelete: false);
        _ch.QueueBind("notifications.token.upsert", _opt.Exchange, "token.upsert");
    }

    public Task PublishAsync(TokenUpsertMessage message, CancellationToken ct = default)
    {
        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        var props = _ch.CreateBasicProperties();
        props.DeliveryMode = 2;
        _ch.BasicPublish(_opt.Exchange, "token.upsert", props, body);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _ch?.Dispose();
        _conn?.Dispose();
    }
}
