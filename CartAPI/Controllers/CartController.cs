using Microsoft.AspNetCore.Mvc;
using CartAPI.Repositories;
using CartAPI.DTO;
using CartAPI.RabbitMQ;

namespace CartAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CartController(ICartRepository cartRepository) : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<ActionResult<CartDTO>> FindById(string userId)
    {
        var cart = cartRepository.FindCartByUserId(userId);
        return cart != null
            ? Ok(cart)
            : new CartDTO { Header = new CartHeaderDTO(), Details = new List<CartDetailDTO>() };
    }

    [HttpPost]
    public async Task<ActionResult<CartDTO>> AddCart(CartDTO dto)
    {
        var cart = cartRepository.SaveOrUpdateCart(dto);
        return cart != null ? Ok(cart) : NotFound();
    }

    [HttpPut]
    public async Task<ActionResult<CartDTO>> UpdateCart(CartDTO dto)
    {
        var cart = cartRepository.SaveOrUpdateCart(dto);
        return cart != null ? Ok(cart) : NotFound();
    }

    [HttpDelete("{detailId}")]
    public async Task<ActionResult> RemoveCart(int detailId)
    {
        var result = cartRepository.RemoveFromCart(detailId);
        return result ? Ok() : BadRequest();
    }

    [HttpPut("apply-coupon")]
    public async Task<ActionResult> ApplyCoupon(CartHeaderDTO headerDTO)
    {
        return cartRepository.ApplyCoupon(headerDTO.UserId, headerDTO.CouponCode) ? Ok() : NotFound();
    }

    [HttpDelete("remove-coupon/{userId}")]
    public async Task<ActionResult> RemoveCoupon(string userId)
    {
        return cartRepository.RemoveCoupon(userId) ? Ok() : NotFound();
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<CheckoutDTO>> Checkout(CheckoutDTO checkout)
    {
        var cart = cartRepository.FindCartByUserId(checkout.UserId);
        if (cart == null) return NotFound();

        // checkout.Details = cart.Details;
        // checkout.DateTime = DateTime.Now;

        // checkoutPublisher.PublishMessage(checkout, "checkout_queue");

        return Ok(checkout);
    }

}
