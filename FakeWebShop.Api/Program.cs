using FakeWebShop.Domain.Services.Interfaces;
using FakeWebShop.Domain.Services.services;
using FakeWebShop.Persistence;
using FakeWebShop.Persistence.Interfaces;
using FakeWebShop.Persistence.Options;
using FakeWebShop.Persistence.repos;

var builder = WebApplication.CreateBuilder(args);

// bind options uit appsettings.json
builder.Services.Configure<ProductRepositoryOptions>(
    builder.Configuration.GetSection(ProductRepositoryOptions.SectionName)
);

// registreer DbContext (zodat DI + EF tooling werkt)
builder.Services.AddDbContext<ProductDbContext>();


// Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductenService, ProductenService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Repositories
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IProductenRepo, ProductenRepo>();
builder.Services.AddScoped<IProductsRepo, ProductsRepo>();

builder.Services.AddControllers();


var app = builder.Build();


app.UseHttpsRedirection();

app.MapControllers();

app.Run();

