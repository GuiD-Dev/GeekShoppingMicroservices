using OrderAPI.RabbitMQ.Messages;
using OrderAPI.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderAPI.RabbitMQ;

public class PaymentConsumer : BackgroundService
{
    private readonly OrderRepository _repository;
    private IChannel _channel;

    public PaymentConsumer(OrderRepository repository)
    {
        _repository = repository;
        var connection = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        }
        .CreateConnectionAsync().Result;

        _channel = connection.CreateChannelAsync().Result;
        _channel.QueueDeclareAsync(queue: "order_payment_result_queue", false, false, false, arguments: null);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (chanel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            var dto = JsonSerializer.Deserialize<UpdatePaymentResultDTO>(content);
            UpdatePaymentStatus(dto).GetAwaiter().GetResult();
            await _channel.BasicAckAsync(evt.DeliveryTag, false);
        };

        await _channel.BasicConsumeAsync("order_payment_result_queue", false, consumer);
    }

    private async Task UpdatePaymentStatus(UpdatePaymentResultDTO dto)
    {
        try
        {
            await _repository.UpdateOrderPaymentStatus(dto.OrderId, dto.Status);
        }
        catch (Exception)
        {
            //TODO: Log Exception
            throw;
        }
    }
}
