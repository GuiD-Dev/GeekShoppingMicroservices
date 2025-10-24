namespace Frontend.ViewModels;

public class CartViewModel
{
    public CartHeaderViewModel Header { get; set; }
    public IEnumerable<CartDetailViewModel> Details { get; set; }
}
