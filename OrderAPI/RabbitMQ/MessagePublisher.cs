using OrderAPI.RabbitMQ.Messages;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OrderAPI.RabbitMQ;

public class MessagePublisher : IMessagePublisher
{
    private IConnection _connection;

    public MessagePublisher()
    {
        _connection = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        }
        .CreateConnectionAsync().Result;
    }

    public async void PublishMessage(BaseMessage message, string queueName)
    {
        using var channel = await _connection.CreateChannelAsync();
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

    private byte[] GetMessageAsByteArray(BaseMessage message)
    {
        var json = JsonSerializer.Serialize(
            (PaymentDTO)message,
            new JsonSerializerOptions { WriteIndented = true }
        );
        return Encoding.UTF8.GetBytes(json);
    }
}