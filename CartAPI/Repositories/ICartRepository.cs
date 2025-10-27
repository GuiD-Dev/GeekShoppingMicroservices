using CartAPI.DTO;

namespace CartAPI.Repositories;

public interface ICartRepository
{
    CartDTO FindCartByUserId(string userId);
    CartDTO SaveOrUpdateCart(CartDTO dto);
    bool RemoveFromCart(long cartDetailId);
    bool ClearCart(string userId);
    bool ApplyCoupon(string userId, string couponCode);
    bool RemoveCoupon(string userId);
}