using AutoMapper;
using CartAPI.DBContext;
using CartAPI.DTO;
using CartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.Repositories;

public class CartRepository(PgSQLContext context, IMapper mapper) : ICartRepository
{
    public CartDTO FindCartByUserId(string userId)
    {
        Cart cart = new() { Header = context.CartHeaders.FirstOrDefault(c => c.UserId == userId) };
        cart.Details = cart.Header != null
            ? context.CartDetails.Where(c => c.CartHeader.Id == cart.Header.Id).ToList()
            : new List<CartDetail>();
        return mapper.Map<CartDTO>(cart);
    }

    public CartDTO SaveOrUpdateCart(CartDTO cartDto)
    {
        var details = new List<CartDetail>();
        var header = context.CartHeaders.FirstOrDefault(c => c.UserId == cartDto.Header.UserId);
        if (header == null)
        {
            header = new CartHeader
            {
                UserId = cartDto.Header.UserId,
                CouponCode = cartDto.Header.CouponCode
            };
            context.CartHeaders.Add(header);

            foreach (var detailDto in cartDto.Details)
            {
                var detail = mapper.Map<CartDetail>(detailDto);
                detail.CartHeader = header;
                context.CartDetails.Add(detail);
                details.Add(detail);
            }
        }
        else
        {
            header.CouponCode = cartDto.Header.CouponCode;
            context.CartHeaders.Update(header);

            foreach (var detailDto in cartDto.Details)
            {
                var detail = context.CartDetails.FirstOrDefault(d => d.CartHeader.Id == header.Id && d.ProductId == detailDto.ProductId);
                if (detail == null)
                {
                    detail = mapper.Map<CartDetail>(detailDto);
                    detail.CartHeader = header;
                    context.CartDetails.Add(detail);
                }
                else
                {
                    detail.Count = detailDto.Count;
                    context.CartDetails.Update(detail);
                }
                details.Add(detail);
            }
        }

        context.SaveChanges();

        return mapper.Map<CartDTO>(new Cart { Header = header, Details = details });
    }

    public bool RemoveFromCart(long detailId)
    {
        try
        {
            var detail = context.CartDetails.Include(d => d.CartHeader)
                                            .FirstOrDefault(d => d.Id == detailId);
            if (detail == null) return false;

            context.CartDetails.Remove(detail);

            var detailsCount = context.CartDetails.Count(d => d.CartHeader.Id == detail.CartHeader.Id);

            if (detailsCount == 1)
                context.CartHeaders.Remove(detail.CartHeader);

            context.SaveChanges();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool ClearCart(string userId)
    {
        try
        {
            var cart = context.CartHeaders.FirstOrDefault(c => c.UserId == userId);
            if (cart == null) return false;

            var details = context.CartDetails.Where(c => c.CartHeader.Id == cart.Id).ToList();
            context.CartDetails.RemoveRange(details);
            context.CartHeaders.Remove(cart);
            context.SaveChanges();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool ApplyCoupon(string userId, string couponCode)
    {
        var header = context.CartHeaders.FirstOrDefault(c => c.UserId == userId);

        if (header == null) return false;

        header.CouponCode = couponCode;
        context.CartHeaders.Update(header);
        context.SaveChanges();
        return true;
    }

    public bool RemoveCoupon(string userId)
    {
        var header = context.CartHeaders.FirstOrDefault(c => c.UserId == userId);

        if (header == null) return false;

        header.CouponCode = null;
        context.CartHeaders.Update(header);
        context.SaveChanges();
        return true;
    }
}