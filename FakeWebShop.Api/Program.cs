using FakeWebShop.Persistence;
using FakeWebShop.Persistence.repos;
using FakeWebShop.Domain.Services;
using FakeWebShop.Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using FakeWebShop.Domain.Services.services;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
 

// DbContext (liefst via appsettings, maar dit werkt nu direct)
var cs = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseMySql(cs, ServerVersion.AutoDetect(cs)));

// Repositories
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IProductenRepo, ProductenRepo>();
// services
builder.Services.AddScoped<IProductService, ProductService>();

// repos
builder.Services.AddScoped<IProductsRepo, ProductsRepo>();
// Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductenService, ProductenService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

 

app.UseHttpsRedirection();

app.MapControllers();

app.Run();