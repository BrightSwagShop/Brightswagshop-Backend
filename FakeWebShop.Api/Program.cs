using System.Text.Json.Serialization;
using FakeWebShop.Domain.Abstractions.Storage;
using FakeWebShop.Domain.Services;
using FakeWebShop.Domain.Services.MongoInterfaces;
using FakeWebShop.Persistence.MongoRepo_s;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using FakeWebShop.Persistence.MongoRepo_s.Options;
using FakeWebShop.Persistence.Supabase;
using FakeWebShop.Persistence.Supabase.SupabaseSettings;
using MongoDB.Driver;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);


// MongoOptions binden (voor IOptions<MongoOptions>)
builder.Services.Configure<MongoOptions>(
    builder.Configuration.GetSection("Mongo"));

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration["Mongo:ConnectionString"]));

// Image Storage
builder.Services.Configure<SupabaseStorageSettings>(
    builder.Configuration.GetSection("Supabase"));

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/v2.0";
        options.Audience = builder.Configuration["AzureAd:ClientId"];
    });

builder.Services.AddAuthorization();
// Repository DI & Service DI 
builder.Services.AddScoped<IMongoProductRepository, MongoProductRepository>();
builder.Services.AddScoped<IMongoProductService, MongoProductService>();

// Supabase storage & Interface
builder.Services.AddScoped<IImageStorage, SupabaseImageStorage>();

// Cors 
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<String[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

 

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

var app = builder.Build();
app.UseCors("AllowFrontend");


app.UseHttpsRedirection();
app.MapControllers();
app.Run();

