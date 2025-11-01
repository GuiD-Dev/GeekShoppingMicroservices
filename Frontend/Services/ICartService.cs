using Frontend.ViewModels;

namespace Frontend.Services;

public interface ICartService
{
    Task<CartViewModel> FindCartByUserId(string userId);
    Task<CartViewModel> AddItemToCart(CartViewModel cart);
    Task<CartViewModel> UpdateCart(CartViewModel cart);
    Task<bool> RemoveFromCart(long cartId);
    Task<bool> ClearCart(string userId);

    Task<bool> ApplyCoupon(CartViewModel cart);
    Task<bool> RemoveCoupon(string userId);

    Task<CartViewModel> Checkout(CartHeaderViewModel header);
}