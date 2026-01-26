using AutoMapper;
using CartAPI.DBContext;
using CartAPI.DTO;
using CartAPI.Models;
using CartAPI.RabbitMQ;
using CartAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PgSQLContext>(options =>
    options.UseNpgsql(builder.Configuration["ConnectionStrings:PgSQlConnectionString"])
);

IMapper mapper = new MapperConfiguration(config =>
{
    config.CreateMap<CartDTO, Cart>().ReverseMap();
    config.CreateMap<CartHeaderDTO, CartHeader>().ReverseMap();
    config.CreateMap<CartDetailDTO, CartDetail>().ReverseMap();
}).CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(config => AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddSingleton<ICheckoutPublisher, CheckoutPublisher>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CartAPI", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CartAPI V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    Task.Delay(10000).Wait();
    var db = scope.ServiceProvider.GetRequiredService<PgSQLContext>();
    db.Database.Migrate();
}

app.Run();
