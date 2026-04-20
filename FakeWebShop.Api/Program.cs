using System.Net;
using System.Text.Json.Serialization;
using FakeWebShop.Domain.Abstractions.Storage;
using FakeWebShop.Domain.Services;
using FakeWebShop.Domain.Services.Interface_s;
using FakeWebShop.Domain.Services.MongoInterfaces;
using FakeWebShop.Domain.Services.MongoUserServices;
using FakeWebShop.Persistence.MongoRepo_s;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using FakeWebShop.Persistence.MongoRepo_s.Options;
using FakeWebShop.Persistence.PublicUserRepo_s;
using FakeWebShop.Persistence.PublicUserRepo_s.MongoInterfaces;
using FakeWebShop.Persistence.Supabase;
using FakeWebShop.Persistence.Supabase.SupabaseSettings;
using FakeWebShop.Api.Security;
using MongoDB.Driver;
using Stripe;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
var builder = WebApplication.CreateBuilder(args);


// MongoOptions binden (voor IOptions<MongoOptions>)
builder.Services.Configure<MongoOptions>(
    builder.Configuration.GetSection("Mongo"));

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration["Mongo:ConnectionString"]));

// Image Storage
builder.Services.Configure<SupabaseStorageSettings>(
    builder.Configuration.GetSection("Supabase"));

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];


// Repository DI 
builder.Services.AddScoped<IMongoProductRepository, MongoProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

// Services DI 
builder.Services.AddScoped<IMongoProductService, MongoProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
// Payment Service DI
builder.Services.AddScoped<IStripeWebhookService, StripeWebhookService>();
builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();
builder.Services.AddScoped<IDiscountService, FakeWebShop.Domain.Services.DiscountService>();

builder.Services.AddScoped<MongoUserService, MongoUserService>();
builder.Services.AddScoped<IMongoUserRepository, MongoUserRepository>();
// Supabase storage & Interface
builder.Services.AddScoped<IImageStorage, SupabaseImageStorage>();

// Cors 
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<String[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins!)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

builder.Services
    .AddAuthentication(HeaderAuthDefaults.Scheme)
    .AddScheme<AuthenticationSchemeOptions, HeaderAuthenticationHandler>(
        HeaderAuthDefaults.Scheme,
        _ => { });

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, JsonAuthorizationMiddlewareResultHandler>();

var app = builder.Build();

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

