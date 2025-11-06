namespace OrderAPI.RabbitMQ.Messages;

public class BaseMessage
{
    public long Id { get; set; }
    public DateTime MessageCreated { get; set; }
}