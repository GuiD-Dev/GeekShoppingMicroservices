using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CartAPI.RabbitMQ;

public class CheckoutPublisher : ICheckoutPublisher
{
    private readonly IConnection connection;

    public CheckoutPublisher()
    {
        connection = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        }
        .CreateConnectionAsync().Result;
    }

    public async void PublishMessage(CheckoutMessage message, string queueName)
    {
        using var channel = await connection.CreateChannelAsync();
        await channel.QueueDeclareAsync(queue: queueName, false, false, false, arguments: null);

        var body = GetMessageAsByteArray(message);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: false,
            basicProperties: new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
            },
            body: body
        );
    }

    private byte[] GetMessageAsByteArray(CheckoutMessage message)
    {
        var json = JsonSerializer.Serialize(
            message,
            options: new() { WriteIndented = true, }
        );

        return Encoding.UTF8.GetBytes(json);
    }
}
