using Microsoft.EntityFrameworkCore;
using FeadBack;
using FeadBack.service;
using FeadBack.service.impl;
using System.Net.Http.Headers;
using FeadBack.config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<IVinCheckService, VinCheckService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:VinCheck:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(15);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddHttpClient<IClientCheckService, ClientCheckService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:ClientCheck:BaseUrl"]!);
    client.Timeout = TimeSpan.FromSeconds(15);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

// Или с конфигурацией из appsettings.json
builder.Services.AddSingleton<IKafkaProducerService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var bootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
    var defaultTopic = configuration["Kafka:DefaultTopic"] ?? "feed-responses";
    
    return new KafkaProducerService(bootstrapServers, defaultTopic);
});
// Чтение конфигурации из appsettings.json
builder.Services.Configure<KafkaProducerConfig>(
    builder.Configuration.GetSection("Kafka:Producer"));
builder.Services.AddScoped<FeedBackService, FeedBackServiceImpl>();
var app = builder.Build();

app.MapControllers();  

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
