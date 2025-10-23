using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductAPI.DBContext;
using ProductAPI.DTO;
using ProductAPI.Models;
using ProductAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["MySQlConnection:MySQlConnectionString"];

builder.Services.AddDbContext<MySQLContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 6)))
);

IMapper mapper = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<Product, ProductDTO>();
    cfg.CreateMap<ProductDTO, Product>();
}).CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(config => AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductAPI", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductAPI V1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    Task.Delay(10000).Wait();
    var db = scope.ServiceProvider.GetRequiredService<MySQLContext>();
    db.Database.Migrate();
}

app.Run();