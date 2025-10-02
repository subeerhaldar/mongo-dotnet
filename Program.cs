using MongoDB.Driver;
using MongoGridApi.Models;
using MongoGridApi.Services;
using MongoGridApi.Repositories;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB configuration
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddScoped(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// Register repositories
builder.Services.AddScoped<IRepository<Category>>(sp => new Repository<Category>(sp.GetRequiredService<IMongoDatabase>(), "categories"));
builder.Services.AddScoped<IRepository<Product>>(sp => new Repository<Product>(sp.GetRequiredService<IMongoDatabase>(), "products"));
builder.Services.AddScoped<IRepository<User>>(sp => new Repository<User>(sp.GetRequiredService<IMongoDatabase>(), "users"));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Register seeder
builder.Services.AddScoped<DatabaseSeeder>();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
