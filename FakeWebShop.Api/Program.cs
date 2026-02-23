using FakeWebShop.Domain.Services;
using FakeWebShop.Domain.Services.MongoInterfaces;
using FakeWebShop.Persistence.MongoRepo_s;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using FakeWebShop.Persistence.MongoRepo_s.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);


// MongoClient registeren (Singleton)
builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration["Mongo:ConnectionString"]));

// Repository DI
builder.Services.AddScoped<IMongoProductRepository, MongoProductRepository>();

// Service DI
builder.Services.AddScoped<IMongoProductService, MongoProductService>();


var app = builder.Build();


app.UseHttpsRedirection();




app.Run();

