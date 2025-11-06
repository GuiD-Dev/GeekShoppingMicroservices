using OrderAPI.Models;
using OrderAPI.RabbitMQ.Messages;
using OrderAPI.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderAPI.RabbitMQ;

public class CheckoutConsumer : BackgroundService
{
    private readonly OrderRepository _repository;
    private readonly IMessagePublisher _messagePublisher;
    private IChannel _channel;

    public CheckoutConsumer(OrderRepository repository, IMessagePublisher messagePublisher)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;

        var connection = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        }
        .CreateConnectionAsync().Result;

        _channel = connection.CreateChannelAsync().Result;
        _channel.QueueDeclareAsync(queue: "checkout_queue", false, false, false, arguments: null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (chanel, evt) =>
        {
            var content = Encoding.UTF8.GetString(evt.Body.ToArray());
            var header = JsonSerializer.Deserialize<CartHeaderDTO>(content);
            ProcessOrder(header).GetAwaiter().GetResult();
            await _channel.BasicAckAsync(evt.DeliveryTag, false); // Remove the message from the queue
        };

        _channel.BasicConsumeAsync("checkout_queue", false, consumer);
        return Task.CompletedTask;
    }

    private async Task ProcessOrder(CartHeaderDTO cart)
    {
        Order order = new()
        {
            UserId = cart.UserId,
            FirstName = cart.FirstName,
            LastName = cart.LastName,
            OrderDetails = new List<OrderDetail>(),
            CardNumber = cart.CardNumber,
            CouponCode = cart.CouponCode,
            CVV = cart.CVV,
            DiscountAmount = cart.DiscountAmount,
            Email = cart.Email,
            ExpiryMonthYear = cart.ExpiryMothYear,
            OrderTime = DateTime.Now,
            PurchaseAmount = cart.PurchaseAmount,
            PaymentStatus = false,
            Phone = cart.Phone,
            DateTime = cart.DateTime
        };

        foreach (var details in cart.Details)
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

        PaymentDTO payment = new()
        {
            Name = order.FirstName + " " + order.LastName,
            CardNumber = order.CardNumber,
            CVV = order.CVV,
            ExpiryMonthYear = order.ExpiryMonthYear,
            OrderId = order.Id,
            PurchaseAmount = order.PurchaseAmount,
            Email = order.Email
        };

        try
        {
            _messagePublisher.PublishMessage(payment, "order_payment_process_queue");
        }
        catch (Exception)
        {
            //TODO: Log Exception
            throw;
        }
    }
}
