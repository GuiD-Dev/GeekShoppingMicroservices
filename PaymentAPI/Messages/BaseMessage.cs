namespace PaymentAPI.RabbitMQ;

public class BaseMessage
{
    public long Id { get; set; }
    public DateTime MessageCreated { get; set; }
}