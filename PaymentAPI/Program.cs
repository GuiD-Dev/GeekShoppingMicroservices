using PaymentAPI.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();

builder.Services.AddHostedService<PaymentConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.Run();
