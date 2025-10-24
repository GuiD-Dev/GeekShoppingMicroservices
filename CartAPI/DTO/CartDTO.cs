namespace CartAPI.DTO;

public class CartDTO
{
    public CartHeaderDTO Header { get; set; }
    public IEnumerable<CartDetailDTO> Details { get; set; }
}