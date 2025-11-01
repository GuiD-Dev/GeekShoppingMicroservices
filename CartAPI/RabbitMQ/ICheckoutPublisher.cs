namespace CartAPI.RabbitMQ;

public interface ICheckoutPublisher
{
    void PublishMessage(CheckoutMessage baseMessage, string queueName);
}