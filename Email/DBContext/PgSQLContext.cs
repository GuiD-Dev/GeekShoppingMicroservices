using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email.Model.Context
{
    public class PgSQLContext : DbContext
    {
        public PgSQLContext(DbContextOptions<PgSQLContext> options) : base(options) {}

        public DbSet<EmailLog> Emails { get; set; }
    }
}