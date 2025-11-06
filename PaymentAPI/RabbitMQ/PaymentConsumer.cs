using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PaymentAPI.RabbitMQ;

public class PaymentConsumer : BackgroundService
{
    private IChannel _channel;
    private IMessagePublisher _messagePublisher;

    public PaymentConsumer(IMessagePublisher messagePublisher)
    {
        _messagePublisher = messagePublisher;

        var connection = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        }
        .CreateConnectionAsync().Result;

        _channel = connection.CreateChannelAsync().Result;
        _channel.QueueDeclareAsync(queue: "order_payment_process_queue", false, false, false, arguments: null);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (chanel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            var message = JsonSerializer.Deserialize<PaymentMessage>(content);
            ProcessPayment(message).GetAwaiter().GetResult();
            await _channel.BasicAckAsync(evt.DeliveryTag, false);
        };

        await _channel.BasicConsumeAsync("order_payment_process_queue", false, consumer);
    }

    private async Task ProcessPayment(PaymentMessage message)
    {
        // TODO: Verify if payment is processed here
        var isProcessed = true;

        UpdatePaymentResultMessage paymentResult = new()
        {
            Status = isProcessed,
            OrderId = message.OrderId,
            Email = message.Email
        };

        try
        {
            _messagePublisher.PublishMessage(paymentResult, "order_payment_result_queue");
        }
        catch (Exception)
        {
            //TODO: Log exception
            throw;
        }
    }
}
