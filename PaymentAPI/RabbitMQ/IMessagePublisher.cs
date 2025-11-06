namespace PaymentAPI.RabbitMQ;

public interface IMessagePublisher
{
    void PublishMessage(BaseMessage baseMessage, string queueName);
}