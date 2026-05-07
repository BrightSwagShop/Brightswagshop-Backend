using System.Security.Claims;
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
using FakeWebShop.Api.Security;
using MongoDB.Driver;
using Stripe;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

// Mongo
builder.Services.Configure<MongoOptions>(
    builder.Configuration.GetSection("Mongo"));

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration["Mongo:ConnectionString"]));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoOptions>>();
    return client.GetDatabase(options.Value.Database);
});

// Supabase
builder.Services.Configure<SupabaseStorageSettings>(
    builder.Configuration.GetSection("Supabase"));

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Authentication
var authBuilder = builder.Services.AddAuthentication();

authBuilder.AddMicrosoftIdentityWebApi(
    builder.Configuration.GetSection("AzureAd"),
    jwtBearerScheme: "AzureAd"
);

authBuilder.AddJwtBearer("CustomJwt", options =>
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
            Encoding.UTF8.GetBytes(jwtKey)),
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };
});
// Admin Only -> [Authorize(Policy = "AdminOnly")]
// UserOnly -> [Authorize(Policy = "UserOnly")]
// Admin & User -> [Authorize(Policy = "UserOrAdmin")]

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.AuthenticationSchemes.Add("AzureAd");
        policy.RequireAuthenticatedUser();
        policy.RequireRole("App.Admin");
    });

    options.AddPolicy("UserOnly", policy =>
    {
        policy.AuthenticationSchemes.Add("CustomJwt");
        policy.RequireAuthenticatedUser();
        policy.RequireRole("User");
    });

    options.AddPolicy("UserOrAdmin", policy =>
    {
        policy.AuthenticationSchemes.Add("CustomJwt");
        policy.AuthenticationSchemes.Add("AzureAd");
        policy.RequireAuthenticatedUser();
        policy.RequireRole("User", "App.Admin");
    });
});

// Repository DI
builder.Services.AddScoped<IMongoProductRepository, MongoProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<IMongoUserRepository, MongoUserRepository>();

// Services DI
builder.Services.AddScoped<IMongoProductService, MongoProductService>();
builder.Services.AddScoped<IMongoUserInterface, MongoUserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IStripeWebhookService, StripeWebhookService>();
builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();
builder.Services.AddScoped<IDiscountService, WebShopDiscountService>();
builder.Services.AddScoped<IImageStorage, SupabaseImageStorage>();
builder.Services.AddScoped<JwtService>();

// CORS
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

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services
    .AddAuthentication(HeaderAuthDefaults.Scheme)
    .AddScheme<AuthenticationSchemeOptions, HeaderAuthenticationHandler>(
        HeaderAuthDefaults.Scheme,
        _ => { });

builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, JsonAuthorizationMiddlewareResultHandler>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();