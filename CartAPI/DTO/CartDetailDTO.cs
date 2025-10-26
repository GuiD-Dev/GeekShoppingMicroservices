namespace CartAPI.DTO;

public class CartDetailDTO
{
    public long Id { get; set; }
    public CartHeaderDTO Cart { get; set; }
    public long ProductId { get; set; }
    public int Count { get; set; }

}