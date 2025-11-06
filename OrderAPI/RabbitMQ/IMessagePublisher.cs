using OrderAPI.RabbitMQ.Messages;

namespace OrderAPI.RabbitMQ;

public interface IMessagePublisher
{
    void PublishMessage(BaseMessage baseMessage, string queueName);
}
