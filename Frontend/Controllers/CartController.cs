using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Frontend.ViewModels;
using Frontend.Services;

namespace Frontend.Controllers;

public class CartController(ICartService cartService, IProductService productService, ICouponService couponService) : Controller
{
    public async Task<IActionResult> CartIndex() => View(await FindUserCart());

    private async Task<CartViewModel> FindUserCart()
    {
        // TODO: adjust when Identity Server will be implemented 
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value ?? "1";

        var cart = await cartService.FindCartByUserId(userId);

        if (cart?.Header != null)
        {
            if (!string.IsNullOrEmpty(cart.Header?.CouponCode))
            {
                var coupon = await couponService.GetCoupon(cart.Header.CouponCode);
                if (coupon?.CouponCode != null)
                {
                    cart.Header.DiscountAmount = coupon.DiscountAmount;
                }
            }

            foreach (var detail in cart.Details)
            {
                detail.Product = await productService.FindProductById(detail.ProductId);
                cart.Header.PurchaseAmount += detail.Product.Price * detail.Count;
            }

            cart.Header.PurchaseAmount -= cart.Header.DiscountAmount;
        }

        return cart;
    }

    public async Task<IActionResult> Remove(int id)
    {
        var response = await cartService.RemoveFromCart(id);

        if (response)
            return RedirectToAction(nameof(CartIndex));

        return View();
    }

    [HttpPost]
    [ActionName("ApplyCoupon")]
    public async Task<IActionResult> ApplyCoupon(CartViewModel model)
    {
        // TODO: adjust when Identity Server will be implemented 
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value ?? "1";

        var response = await cartService.ApplyCoupon(model);
        if (response)
            return RedirectToAction(nameof(CartIndex));

        return View();
    }

    [HttpPost]
    [ActionName("RemoveCoupon")]
    public async Task<IActionResult> RemoveCoupon()
    {
        // TODO: adjust when Identity Server will be implemented 
        var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value ?? "1";

        var response = await cartService.RemoveCoupon(userId);
        if (response)
            return RedirectToAction(nameof(CartIndex));

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
