using Microsoft.EntityFrameworkCore;
using OrderAPI.DBContext;
using OrderAPI.RabbitMQ;
using OrderAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["MySQlConnection:MySQlConnectionString"];
var dbContextBuilder = new DbContextOptionsBuilder<MySQLContext>();
dbContextBuilder.UseMySql(
    connectionString, new MySqlServerVersion(new Version(8, 4, 6))
);
builder.Services.AddDbContext<MySQLContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 6)))
);

builder.Services.AddSingleton(new OrderRepository(dbContextBuilder.Options));
builder.Services.AddHostedService<CheckoutConsumer>();

builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();
builder.Services.AddHostedService<PaymentConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    Task.Delay(10000).Wait();
    var db = scope.ServiceProvider.GetRequiredService<MySQLContext>();
    db.Database.Migrate();
}

app.Run();
