using CartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.DBContext;

public class MySQLContext : DbContext
{
    public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) { }

    public DbSet<CartHeader> CartHeaders { get; set; }
    public DbSet<CartDetail> CartDetails { get; set; }
}