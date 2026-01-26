using CartAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CartAPI.DBContext;

public class PgSQLContext : DbContext
{
    public PgSQLContext(DbContextOptions<PgSQLContext> options) : base(options) { }

    public DbSet<CartHeader> CartHeaders { get; set; }
    public DbSet<CartDetail> CartDetails { get; set; }
}