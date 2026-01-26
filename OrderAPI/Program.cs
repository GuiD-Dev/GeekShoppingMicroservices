using Microsoft.EntityFrameworkCore;
using OrderAPI.DBContext;
using OrderAPI.RabbitMQ;
using OrderAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["ConnectionStrings:PgSQlConnectionString"];
var optionsBuilder = new DbContextOptionsBuilder<PgSQLContext>();
optionsBuilder.UseNpgsql(connectionString);

builder.Services.AddDbContext<PgSQLContext>(options =>
    options.UseNpgsql(builder.Configuration["ConnectionStrings:PgSQlConnectionString"])
);

builder.Services.AddSingleton(new OrderRepository(optionsBuilder.Options));
builder.Services.AddHostedService<CheckoutConsumer>();

builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();
builder.Services.AddHostedService<PaymentConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    Task.Delay(10000).Wait();
    var db = scope.ServiceProvider.GetRequiredService<PgSQLContext>();
    db.Database.Migrate();
}

app.Run();
