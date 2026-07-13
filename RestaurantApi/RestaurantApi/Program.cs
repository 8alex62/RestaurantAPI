using RestaurantApi.Models;
using RestaurantApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<OrderRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var repository = app.Services.GetRequiredService<OrderRepository>();

app.MapGet("/", () => "Restaurant API is running. See /swagger for details.");

// Example endpoint to illustrate routing and repository usage.
// This is NOT a reference implementation, just a starting point.
app.MapGet("/api/orders", () => Results.Ok(repository.GetAll()));

app.Run();
