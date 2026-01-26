using AutoMapper;
using CouponAPI.DBContext;
using CouponAPI.DTO;
using Microsoft.EntityFrameworkCore;

namespace CouponAPI.Repositories;

public class CouponRepository(PgSQLContext context, IMapper mapper) : ICouponRepository
{
    public async Task<CouponDTO> GetCouponByCode(string couponCode)
    {
        var coupon = await context.Coupons.FirstOrDefaultAsync(c => c.CouponCode == couponCode);
        return mapper.Map<CouponDTO>(coupon);
    }
}