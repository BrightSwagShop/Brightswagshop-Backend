using System.Text;
using System.Text.Json.Serialization;
using FakeWebShop.Domain.Abstractions.Storage;
using FakeWebShop.Domain.Services;
using FakeWebShop.Domain.Services.MongoInterface_s;
using FakeWebShop.Domain.Services.MongoInterfaces;
using FakeWebShop.Domain.Services.MongoUserServices;
using FakeWebShop.Persistence.MongoRepo_s;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using FakeWebShop.Persistence.MongoRepo_s.Options;
using FakeWebShop.Persistence.PublicUserRepo_s;
using FakeWebShop.Persistence.PublicUserRepo_s.MongoInterfaces;
using FakeWebShop.Persistence.Supabase;
using FakeWebShop.Persistence.Supabase.SupabaseSettings;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);


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


// Repository DI & Service DI 
builder.Services.AddScoped<IMongoProductRepository, MongoProductRepository>();
builder.Services.AddScoped<IMongoProductService, MongoProductService>();
builder.Services.AddScoped<MongoUserService, MongoUserService>();
builder.Services.AddScoped<IMongoUserRepository, MongoUserRepository>();
// Supabase storage & Interface
builder.Services.AddScoped<IImageStorage, SupabaseImageStorage>();
 builder.Services.AddScoped<IFavoriteRepository,FavoriteRepository>();
 builder.Services.AddScoped<IFavoriteService,FavoriteService>();
builder.Services.AddScoped<JwtService>();
//jwt bearer injecteren
 builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "yourapp",
            ValidAudience = "yourapp",

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("YOUR_SECRET_KEY_HIER_MIN_16_CHARS")
            )
        };
    });

 
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

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

