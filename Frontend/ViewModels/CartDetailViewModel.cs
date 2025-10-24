namespace Frontend.ViewModels;

public class CartDetailViewModel
{
    public long Id { get; set; }
    public CartHeaderViewModel Cart { get; set; }
    public ProductViewModel Product { get; set; }
    public int Count { get; set; }
}