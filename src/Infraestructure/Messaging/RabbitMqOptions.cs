namespace UsersService.Src.Infraestructure.Messaging;

public sealed class RabbitMqOptions
{
    public string ConnectionString { get; set; } = string.Empty;

    public string Exchange { get; set; } = "spacio.direct";
}
