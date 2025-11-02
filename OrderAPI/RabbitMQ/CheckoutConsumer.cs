using OrderAPI.Models;
using OrderAPI.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderAPI.RabbitMQ;

public class CheckoutConsumer : BackgroundService
{
    private readonly OrderRepository _repository;
    private IConnection _connection;
    private IChannel _channel;

    public CheckoutConsumer(OrderRepository repository)
    {
        _repository = repository;

        _connection = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        }
        .CreateConnectionAsync().Result;

        _channel = _connection.CreateChannelAsync().Result;
        _channel.QueueDeclareAsync(queue: "checkout_queue", false, false, false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += (chanel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            CheckoutHeaderDTO header = JsonSerializer.Deserialize<CheckoutHeaderDTO>(content);
            ProcessOrder(header).GetAwaiter().GetResult();
            _channel.BasicAckAsync(evt.DeliveryTag, false); // Remove the message from the queue
            return Task.CompletedTask;
        };

        _channel.BasicConsumeAsync("checkout_queue", false, consumer);
        return Task.CompletedTask;
    }

    private async Task ProcessOrder(CheckoutHeaderDTO dto)
    {
        Order order = new()
        {
            UserId = dto.UserId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            OrderDetails = new List<OrderDetail>(),
            CardNumber = dto.CardNumber,
            CouponCode = dto.CouponCode,
            CVV = dto.CVV,
            DiscountAmount = dto.DiscountAmount,
            Email = dto.Email,
            ExpiryMonthYear = dto.ExpiryMothYear,
            OrderTime = DateTime.Now,
            PurchaseAmount = dto.PurchaseAmount,
            PaymentStatus = false,
            Phone = dto.Phone,
            DateTime = dto.DateTime
        };

        foreach (var details in dto.Details)
        {
            OrderDetail detail = new()
            {
                ProductId = details.ProductId,
                // TODO: Refactor null references
                ProductName = details.Product?.Name ?? string.Empty,
                Price = details.Product?.Price ?? 0,
                Count = details.Count,
            };
            order.CartTotalItens += details.Count;
            order.OrderDetails.Add(detail);
        }

        await _repository.AddOrder(order);
    }
}
