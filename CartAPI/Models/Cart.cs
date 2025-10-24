namespace CartAPI.Models;

public class Cart
{
    public CartHeader Header { get; set; }
    public IEnumerable<CartDetail> Details { get; set; }
}
