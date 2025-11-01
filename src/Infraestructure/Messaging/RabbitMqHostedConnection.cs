using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace UsersService.Src.Infraestructure.Messaging;

public interface IRabbitMqChannelAccessor
{
    IModel Channel { get; }

    string Exchange { get; }
}

public sealed class RabbitMqHostedConnection : IHostedService, IDisposable, IRabbitMqChannelAccessor
{
    private readonly RabbitMqOptions _opt;
    private readonly ILogger<RabbitMqHostedConnection> _log;
    private IConnection? _conn;
    private IModel? _ch;

    public RabbitMqHostedConnection(
        IOptions<RabbitMqOptions> opt,
        ILogger<RabbitMqHostedConnection> log
    )
    {
        _opt = opt.Value;
        _log = log;
    }

    public IModel Channel =>
        _ch ?? throw new InvalidOperationException("RabbitMQ channel not initialized");

    public string Exchange => _opt.Exchange;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_opt.ConnectionString),
            DispatchConsumersAsync = true,
        };
        _conn = factory.CreateConnection();
        _ch = _conn.CreateModel();

        _ch.ExchangeDeclare(_opt.Exchange, ExchangeType.Direct, durable: true, autoDelete: false);

        // ⚠️ DECLARA COLA CON NOMBRE EXPLÍCITO
        _ch.QueueDeclare(
            queue: "notifications.token.upsert",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
        _ch.QueueBind(
            queue: "notifications.token.upsert",
            exchange: _opt.Exchange,
            routingKey: "token.upsert"
        );

        _log.LogInformation(
            "✅ RabbitMQ connected and queue declared (exchange: {Exchange})",
            _opt.Exchange
        );
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _log.LogInformation("🧹 Closing RabbitMQ connection...");
        _ch?.Close();
        _conn?.Close();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _ch?.Dispose();
        _conn?.Dispose();
    }
}
