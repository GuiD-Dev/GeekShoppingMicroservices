using Microsoft.EntityFrameworkCore;
using OrderAPI.Models;

namespace OrderAPI.DBContext;

public class PgSQLContext : DbContext
{
    public PgSQLContext(DbContextOptions<PgSQLContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

}
