using System.Text;
using System.Text.Json.Serialization;
using FakeWebShop.Domain.Abstractions.Storage;
using FakeWebShop.Domain.Services;
using FakeWebShop.Domain.Services.Interface_s;
using FakeWebShop.Domain.Services.MongoInterfaces;
using FakeWebShop.Domain.Services.MongoUserServices;
using FakeWebShop.Domain.Services.MongoUserServices.MongoInterfaces;
using FakeWebShop.Persistence.MongoRepo_s;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using FakeWebShop.Persistence.MongoRepo_s.Options;
using FakeWebShop.Persistence.PublicUserRepo_s;
using FakeWebShop.Persistence.PublicUserRepo_s.MongoInterfaces;
using FakeWebShop.Persistence.Supabase;
using FakeWebShop.Persistence.Supabase.SupabaseSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;


// MongoOptions binden (voor IOptions<MongoOptions>)
builder.Services.Configure<MongoOptions>(
    builder.Configuration.GetSection("Mongo"));

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration["Mongo:ConnectionString"]));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var options = sp.GetRequiredService<
        Microsoft.Extensions.Options.IOptions<MongoOptions>>();

    return client.GetDatabase(options.Value.Database);
});

// Image Storage
builder.Services.Configure<SupabaseStorageSettings>(
    builder.Configuration.GetSection("Supabase"));

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

// Repository DI
builder.Services.AddScoped<IMongoProductRepository, MongoProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

// Services DI
builder.Services.AddScoped<IMongoProductService, MongoProductService>();
builder.Services.AddScoped<IMongoUserInterface, MongoUserService>();
builder.Services.AddScoped<MongoUserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IStripeWebhookService, StripeWebhookService>();
builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();
builder.Services.AddScoped<IDiscountService, WebShopDiscountService>();

builder.Services.AddScoped<MongoUserService, MongoUserService>();
builder.Services.AddScoped<IMongoUserRepository, MongoUserRepository>();
builder.Services.AddScoped<IImageStorage, SupabaseImageStorage>();


builder.Services.AddScoped<MongoUserService, MongoUserService>();
builder.Services.AddScoped<IMongoUserRepository, MongoUserRepository>();
builder.Services.AddScoped<JwtService>();

// Supabase storage & Interface
builder.Services.AddScoped<IImageStorage, SupabaseImageStorage>();


//jwt bearer injecteren
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            )
        };
    });

builder.Services.AddAuthorization();

// Cors 
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

app.MapControllers();

app.Run();